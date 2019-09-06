using System.Collections.Generic;

namespace RTLTMPro
{
    public static class LigatureFixer
    {
        private static readonly List<char> LtrTextHolder = new List<char>(512);

        /// <summary>
        ///     Fixes the flow of the text.
        /// </summary>
        public static void Fix(FastStringBuilder input, FastStringBuilder output, bool farsi, bool fixTextTags, bool preserveNumbers)
        {
            // Some texts like tags, English words and numbers need to be displayed in their original order.
            // This list keeps the characters that their order should be reserved and streams reserved texts into final letters.
            LtrTextHolder.Clear();
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

                if (char.IsPunctuation(characterAtThisIndex) || char.IsSymbol(characterAtThisIndex))
                {
                    if (fixTextTags)
                    {
                        if (characterAtThisIndex == '>')
                        {
                            // We need to check if it is actually the beginning of a tag.
                            bool isValidTag = false;
                            // If > is at the end of the text (At beginning of the array), it can't be a tag
                            if (isAtEnd == false)
                            {
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    // Tags do not have space inside
                                    if (input.Get(j) == ' ')
                                    {
                                        break;
                                    }

                                    // Tags do not have RTL characters inside
                                    if (TextUtils.IsRTLCharacter(input.Get(j)))
                                    {
                                        break;
                                    }

                                    if (input.Get(j) == '<')
                                    {
                                        isValidTag = true;
                                        break;
                                    }
                                }
                            }

                            if (LtrTextHolder.Count > 0 && isValidTag)
                            {
                                for (int j = 0; j < LtrTextHolder.Count; j++)
                                {
                                    output.Append(LtrTextHolder[LtrTextHolder.Count - 1 - j]);
                                }

                                LtrTextHolder.Clear();
                            }
                        }
                    }

                    if (characterAtThisIndex == ')')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = '(';
                            }
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                            if (isAfterRTLCharacter)
                            {
                                characterAtThisIndex = '(';
                            }
                        }
                        else if (isAtBeginning)
                        {
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                            if (isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = '(';
                            }
                        }
                    }
                    else if (characterAtThisIndex == '(')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = ')';
                            }
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                            if (isAfterRTLCharacter)
                            {
                                characterAtThisIndex = ')';
                            }
                        }
                        else if (isAtBeginning)
                        {
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                            if (isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = ')';
                            }
                        }
                    }
                    else if (characterAtThisIndex == '«')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = '»';
                            }
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                            if (isAfterRTLCharacter)
                            {
                                characterAtThisIndex = '»';
                            }
                        }
                        else if (isAtBeginning)
                        {
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                            if (isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = '»';
                            }
                        }
                    }
                    else if (characterAtThisIndex == '»')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = '«';
                            }
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = TextUtils.IsRTLCharacter(nextCharacter);
                            if (isAfterRTLCharacter)
                            {
                                characterAtThisIndex = '«';
                            }
                        }
                        else if (isAtBeginning)
                        {
                            bool isBeforeRTLCharacter = TextUtils.IsRTLCharacter(previousCharacter);
                            if (isBeforeRTLCharacter)
                            {
                                characterAtThisIndex = '«';
                            }
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
                            if (LtrTextHolder.Count > 0)
                            {
                                for (int j = 0; j < LtrTextHolder.Count; j++)
                                {
                                    output.Append(LtrTextHolder[LtrTextHolder.Count - 1 - j]);
                                }

                                LtrTextHolder.Clear();
                            }

                            output.Append(characterAtThisIndex);
                        }
                        else
                        {
                            LtrTextHolder.Add(characterAtThisIndex);
                        }
                    }
                    else if (isAtEnd)
                    {
                        LtrTextHolder.Add(characterAtThisIndex);
                    }
                    else if (isAtBeginning)
                    {
                        output.Append(characterAtThisIndex);
                    }

                    if (fixTextTags)
                    {
                        if (characterAtThisIndex == '<')
                        {
                            bool valid = false;

                            if (isAtBeginning == false)
                            {
                                for (int j = i + 1; j < input.Length; j++)
                                {
                                    // Tags do not have space inside
                                    if (input.Get(j) == ' ')
                                    {
                                        break;
                                    }

                                    // Tags do not have RTL characters inside
                                    if (TextUtils.IsRTLCharacter(input.Get(j)))
                                    {
                                        break;
                                    }

                                    if (input.Get(j) == '>')
                                    {
                                        valid = true;
                                        break;
                                    }
                                }
                            }

                            if (LtrTextHolder.Count > 0 && valid)
                            {
                                for (int j = 0; j < LtrTextHolder.Count; j++)
                                {
                                    output.Append(LtrTextHolder[LtrTextHolder.Count - 1 - j]);
                                }

                                LtrTextHolder.Clear();
                            }
                        }
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

                if (characterAtThisIndex >= (char)0xD800 &&
                    characterAtThisIndex <= (char)0xDBFF ||
                    characterAtThisIndex >= (char)0xDC00 && characterAtThisIndex <= (char)0xDFFF)
                {
                    LtrTextHolder.Add(characterAtThisIndex);
                    continue;
                }

                if (LtrTextHolder.Count > 0)
                {
                    for (int j = 0; j < LtrTextHolder.Count; j++)
                    {
                        output.Append(LtrTextHolder[LtrTextHolder.Count - 1 - j]);
                    }

                    LtrTextHolder.Clear();
                }

                if (characterAtThisIndex != 0xFFFF &&
                    characterAtThisIndex != (int)GeneralLetters.ZeroWidthNoJoiner)
                {
                    output.Append(characterAtThisIndex);
                }
            }

            if (LtrTextHolder.Count > 0)
            {
                for (int j = 0; j < LtrTextHolder.Count; j++)
                {
                    output.Append(LtrTextHolder[LtrTextHolder.Count - 1 - j]);
                }

                LtrTextHolder.Clear();
            }
        }
    }
}