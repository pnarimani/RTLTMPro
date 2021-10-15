using System.Collections.Generic;

namespace RTLTMPro
{
    public static class LigatureFixer
    {
        private static readonly List<char> LtrTextHolder = new List<char>(512);
        private static readonly List<char> TagTextHolder = new List<char>(512);
        private static readonly Dictionary<char, char> MirroredCharsMap = new Dictionary<char, char>()
        {
            ['('] = ')',
            [')'] = '(',
            ['»'] = '«',
            ['«'] = '»',
        };
        private static readonly HashSet<char> MirroredCharsSet = new HashSet<char>(MirroredCharsMap.Keys);
        private static void FlushBufferToOutput(List<char> buffer, FastStringBuilder output)
        {
            for (int j = 0; j < buffer.Count; j++)
            {
                output.Append(buffer[buffer.Count - 1 - j]);
            }

            buffer.Clear();
        }

        /// <summary>
        ///     Fixes the flow of the text.
        /// </summary>
        public static void Fix(FastStringBuilder input, FastStringBuilder output, bool farsi, bool fixTextTags, bool preserveNumbers)
        {
            // Some texts like tags, English words and numbers need to be displayed in their original order.
            // This list keeps the characters that their order should be reserved and streams reserved texts into final letters.
            LtrTextHolder.Clear();
            TagTextHolder.Clear();
            for (int i = input.Length - 1; i >= 0; i--)
            {
                bool isInMiddle = i > 0 && i < input.Length - 1;
                bool isAtBeginning = i == 0;
                bool isAtEnd = i == input.Length - 1;

                char characterAtThisIndex = input.Get(i);

                char nextCharacter = default;
                if (!isAtEnd)
                    nextCharacter = input.Get(i + 1);

                char previousCharacter = default;
                if (!isAtBeginning)
                    previousCharacter = input.Get(i - 1);

                if (fixTextTags)
                {
                    if (characterAtThisIndex == '>')
                    {
                        // We need to check if it is actually the beginning of a tag.
                        bool isValidTag = false;
                        int nextI = i;
                        TagTextHolder.Add(characterAtThisIndex);

                        for (int j = i - 1; j >= 0; j--)
                        {
                            var jChar = input.Get(j);
                            
                            TagTextHolder.Add(jChar);

                            if (jChar == '<')
                            {
                                var jPlus1Char = input.Get(j + 1);
                                // Tags do not start with space
                                if (jPlus1Char == ' ')
                                {
                                    break;
                                }
                                isValidTag = true;
                                nextI = j;
                                break;
                            }
                        }

                        if (isValidTag)
                        {
                            FlushBufferToOutput(LtrTextHolder, output);
                            FlushBufferToOutput(TagTextHolder, output);
                            i = nextI;
                            continue;
                        } else
                        {
                            TagTextHolder.Clear();
                        }
                    }
                }

                if (char.IsPunctuation(characterAtThisIndex) || char.IsSymbol(characterAtThisIndex))
                {

                    if (MirroredCharsSet.Contains(characterAtThisIndex))
                    {
                        // IsRTLCharacter returns false for null
                        bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                        bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);

                        if (isAfterRTLCharacter || isBeforeRTLCharacter)
                        {
                            characterAtThisIndex = MirroredCharsMap[characterAtThisIndex];
                        }
                    }

                    if (isInMiddle)
                    {
                        bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                        bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                        bool isBeforeWhiteSpace = char.IsWhiteSpace(nextCharacter);
                        bool isAfterWhiteSpace = char.IsWhiteSpace(previousCharacter);
                        bool isUnderline = characterAtThisIndex == '_';
                        bool isSpecialPunctuation = characterAtThisIndex == '.' ||
                                                    characterAtThisIndex == '،' ||
                                                    characterAtThisIndex == '؛';

                        if (isBeforeRTLCharacter && isAfterRTLCharacter ||
                            isAfterWhiteSpace && isSpecialPunctuation ||
                            isBeforeWhiteSpace && isAfterRTLCharacter ||
                            isBeforeRTLCharacter && isAfterWhiteSpace ||
                            (isBeforeRTLCharacter || isAfterRTLCharacter) && isUnderline)
                        {
                            FlushBufferToOutput(LtrTextHolder, output);
                            output.Append(characterAtThisIndex);
                        } else
                        {
                            LtrTextHolder.Add(characterAtThisIndex);
                        }
                    } else if (isAtEnd)
                    {
                        LtrTextHolder.Add(characterAtThisIndex);
                    } else if (isAtBeginning)
                    {
                        output.Append(characterAtThisIndex);
                    }

                    continue;
                }

                if (isInMiddle)
                {
                    bool isAfterEnglishChar = TextUtils.IsEnglishLetter(previousCharacter);
                    bool isBeforeEnglishChar = TextUtils.IsEnglishLetter(nextCharacter);
                    bool isAfterNumber = TextUtils.IsNumber(previousCharacter, preserveNumbers, farsi);
                    bool isBeforeNumber = TextUtils.IsNumber(nextCharacter, preserveNumbers, farsi);
                    bool isAfterSymbol = char.IsSymbol(previousCharacter);
                    bool isBeforeSymbol = char.IsSymbol(nextCharacter);

                    // For cases where english words and farsi/arabic are mixed. This allows for using farsi/arabic, english and numbers in one sentence.
                    // If the space is between numbers,symbols or English words, keep the order
                    if (characterAtThisIndex == ' ' &&
                        (isBeforeEnglishChar || isBeforeNumber || isBeforeSymbol) &&
                        (isAfterEnglishChar || isAfterNumber || isAfterSymbol))
                    {
                        LtrTextHolder.Add(characterAtThisIndex);
                        continue;
                    }
                }

                if (TextUtils.IsEnglishLetter(characterAtThisIndex) ||
                    TextUtils.IsNumber(characterAtThisIndex, preserveNumbers, farsi))
                {
                    LtrTextHolder.Add(characterAtThisIndex);
                    continue;
                }

                // handle surrogates
                if (characterAtThisIndex >= (char)0xD800 &&
                    characterAtThisIndex <= (char)0xDBFF ||
                    characterAtThisIndex >= (char)0xDC00 && characterAtThisIndex <= (char)0xDFFF)
                {
                    LtrTextHolder.Add(characterAtThisIndex);
                    continue;
                }

                FlushBufferToOutput(LtrTextHolder, output);

                if (characterAtThisIndex != 0xFFFF &&
                    characterAtThisIndex != (int)ArabicGeneralLetters.ZeroWidthNoJoiner)
                {
                    output.Append(characterAtThisIndex);
                }
            }

            FlushBufferToOutput(LtrTextHolder, output);
        }
    }
}