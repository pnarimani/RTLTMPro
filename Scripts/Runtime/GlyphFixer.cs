using System.Collections.Generic;

namespace RTLTMPro
{
    public static class GlyphFixer
    {
        public static Dictionary<char, char> EnglishToFarsiNumberMap = new Dictionary<char, char>()
        {
            [(char)EnglishNumbers.Zero] = (char)FarsiNumbers.Zero,
            [(char)EnglishNumbers.One] = (char)FarsiNumbers.One,
            [(char)EnglishNumbers.Two] = (char)FarsiNumbers.Two,
            [(char)EnglishNumbers.Three] = (char)FarsiNumbers.Three,
            [(char)EnglishNumbers.Four] = (char)FarsiNumbers.Four,
            [(char)EnglishNumbers.Five] = (char)FarsiNumbers.Five,
            [(char)EnglishNumbers.Six] = (char)FarsiNumbers.Six,
            [(char)EnglishNumbers.Seven] = (char)FarsiNumbers.Seven,
            [(char)EnglishNumbers.Eight] = (char)FarsiNumbers.Eight,
            [(char)EnglishNumbers.Nine] = (char)FarsiNumbers.Nine,
        };

        public static Dictionary<char, char> EnglishToHinduNumberMap = new Dictionary<char, char>()
        {
            [(char)EnglishNumbers.Zero] = (char)HinduNumbers.Zero,
            [(char)EnglishNumbers.One] = (char)HinduNumbers.One,
            [(char)EnglishNumbers.Two] = (char)HinduNumbers.Two,
            [(char)EnglishNumbers.Three] = (char)HinduNumbers.Three,
            [(char)EnglishNumbers.Four] = (char)HinduNumbers.Four,
            [(char)EnglishNumbers.Five] = (char)HinduNumbers.Five,
            [(char)EnglishNumbers.Six] = (char)HinduNumbers.Six,
            [(char)EnglishNumbers.Seven] = (char)HinduNumbers.Seven,
            [(char)EnglishNumbers.Eight] = (char)HinduNumbers.Eight,
            [(char)EnglishNumbers.Nine] = (char)HinduNumbers.Nine,
        };


        /// <summary>
        ///     Fixes the shape of letters based on their position.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="preserveNumbers"></param>
        /// <param name="farsi"></param>
        /// <returns></returns>
        public static void Fix(FastStringBuilder input, FastStringBuilder output, bool preserveNumbers, bool farsi, bool fixTextTags)
        {
            FixYah(input, farsi);

            output.SetValue(input);

            for (int i = 0; i < input.Length; i++)
            {
                bool skipNext = false;
                char iChar = input.Get(i);

                // For special Lam Letter connections.
                if (iChar == (char)GeneralLetters.Lam)
                {
                    if (i < input.Length - 1)
                    {
                        skipNext = HandleSpecialLam(input, output, i);
                        if (skipNext)
                            iChar = output.Get(i);
                    }
                }

                // We don't want to fix tatweel or zwnj character
                if (iChar == (int)GeneralLetters.ArabicTatweel ||
                    iChar == (int)GeneralLetters.ZeroWidthNoJoiner)
                {
                    continue;
                }

                if (TextUtils.IsRTLCharacter(iChar))
                {
                    char converted = GlyphTable.Convert(iChar);

                    if (IsMiddleLetter(input, i))
                    {
                        output.Set(i, (char)(converted + 3));
                    } else if (IsFinishingLetter(input, i))
                    {
                        output.Set(i, (char)(converted + 1));
                    } else if (IsLeadingLetter(input, i))
                    {
                        output.Set(i, (char)(converted + 2));
                    }
                }

                // If this letter as Lam and special Lam-Alef connection was made, We want to skip the Alef
                // (Lam-Alef occupies 1 space)
                if (skipNext)
                {
                    i++;
                }
            }

            if (!preserveNumbers)
            {
                if (fixTextTags)
                {
                    FixNumbersOutsideOfTags(output, farsi);
                } else
                {
                    FixNumbers(output, farsi);
                }
            }
        }

        /// <summary>
        ///     Removes tashkeel. Converts general RTL letters to isolated form. Also fixes Farsi and Arabic ی letter.
        /// </summary>
        /// <param name="text">Input to prepare</param>
        /// <param name="farsi"></param>
        /// <returns>Prepared input in char array</returns>
        public static void FixYah(FastStringBuilder text, bool farsi)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (farsi && text.Get(i) == (int)GeneralLetters.Ya)
                {
                    text.Set(i, (char)GeneralLetters.PersianYa);
                } else if (farsi == false && text.Get(i) == (int)GeneralLetters.PersianYa)
                {
                    text.Set(i, (char)GeneralLetters.Ya);
                }
            }
        }

        /// <summary>
        ///     Handles the special Lam-Alef connection in the text.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <param name="i">Index of Lam letter</param>
        /// <returns><see langword="true" /> if special connection has been made.</returns>
        private static bool HandleSpecialLam(FastStringBuilder input, FastStringBuilder output, int i)
        {
            bool isFixed;
            switch (input.Get(i + 1))
            {
                case (char)GeneralLetters.AlefMaksoor:
                    output.Set(i, (char)0xFEF7);
                    isFixed = true;
                    break;
                case (char)GeneralLetters.Alef:
                    output.Set(i, (char)0xFEF9);
                    isFixed = true;
                    break;
                case (char)GeneralLetters.AlefHamza:
                    output.Set(i, (char)0xFEF5);
                    isFixed = true;
                    break;
                case (char)GeneralLetters.AlefMad:
                    output.Set(i, (char)0xFEF3);
                    isFixed = true;
                    break;
                default:
                    isFixed = false;
                    break;
            }

            if (isFixed)
            {
                output.Set(i + 1, (char)0xFFFF);
            }

            return isFixed;
        }

        /// <summary>
        ///     Converts English numbers to Persian or Arabic numbers.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="farsi"></param>
        /// <returns>Converted number</returns>
        public static void FixNumbers(FastStringBuilder text, bool farsi)
        {
            text.Replace((char)EnglishNumbers.Zero, farsi ? (char)FarsiNumbers.Zero : (char)HinduNumbers.Zero);
            text.Replace((char)EnglishNumbers.One, farsi ? (char)FarsiNumbers.One : (char)HinduNumbers.One);
            text.Replace((char)EnglishNumbers.Two, farsi ? (char)FarsiNumbers.Two : (char)HinduNumbers.Two);
            text.Replace((char)EnglishNumbers.Three, farsi ? (char)FarsiNumbers.Three : (char)HinduNumbers.Three);
            text.Replace((char)EnglishNumbers.Four, farsi ? (char)FarsiNumbers.Four : (char)HinduNumbers.Four);
            text.Replace((char)EnglishNumbers.Five, farsi ? (char)FarsiNumbers.Five : (char)HinduNumbers.Five);
            text.Replace((char)EnglishNumbers.Six, farsi ? (char)FarsiNumbers.Six : (char)HinduNumbers.Six);
            text.Replace((char)EnglishNumbers.Seven, farsi ? (char)FarsiNumbers.Seven : (char)HinduNumbers.Seven);
            text.Replace((char)EnglishNumbers.Eight, farsi ? (char)FarsiNumbers.Eight : (char)HinduNumbers.Eight);
            text.Replace((char)EnglishNumbers.Nine, farsi ? (char)FarsiNumbers.Nine : (char)HinduNumbers.Nine);
        }

        /// <summary>
        ///     Converts English numbers that are outside tags to Persian or Arabic numbers.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="farsi"></param>
        /// <returns>Text with converted numbers</returns>
        public static void FixNumbersOutsideOfTags(FastStringBuilder text, bool farsi)
        {
            var englishDigits = new HashSet<char>(EnglishToFarsiNumberMap.Keys);
            for (int i = 0; i < text.Length; i++)
            {
                var iChar = text.Get(i);
                // skip valid tags
                if (iChar == '<')
                {
                    bool sawValidTag = false;
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        char jChar = text.Get(j);
                        if ((j == i + 1 && jChar == ' ') || jChar == '<')
                        {
                            break;
                        } else if (jChar == '>')
                        {
                            i = j;
                            sawValidTag = true;
                            break;
                        }
                    }

                    if (sawValidTag) continue;
                }

                if (englishDigits.Contains(iChar))
                {
                    text.Set(i, farsi ? EnglishToFarsiNumberMap[iChar] : EnglishToHinduNumberMap[iChar]);
                }
            }
        }

        /// <summary>
        ///     Is the letter at provided index a leading letter?
        /// </summary>
        /// <returns><see langword="true" /> if the letter is a leading letter</returns>
        private static bool IsLeadingLetter(FastStringBuilder letters, int index)
        {
            var currentIndexLetter = letters.Get(index);

            char previousIndexLetter = default;
            if (index != 0)
                previousIndexLetter = letters.Get(index - 1);

            char nextIndexLetter = default;
            if (index < letters.Length - 1)
                nextIndexLetter = letters.Get(index + 1);

            bool isPreviousLetterNonConnectable = index == 0 ||
                                                  !TextUtils.IsRTLCharacter(previousIndexLetter) ||
                                                  previousIndexLetter == (int)GeneralLetters.Alef ||
                                                  previousIndexLetter == (int)GeneralLetters.Dal ||
                                                  previousIndexLetter == (int)GeneralLetters.Thal ||
                                                  previousIndexLetter == (int)GeneralLetters.Ra2 ||
                                                  previousIndexLetter == (int)GeneralLetters.Zeen ||
                                                  previousIndexLetter == (int)GeneralLetters.PersianZe ||
                                                  previousIndexLetter == (int)GeneralLetters.Waw ||
                                                  previousIndexLetter == (int)GeneralLetters.AlefMad ||
                                                  previousIndexLetter ==
                                                  (int)GeneralLetters.AlefHamza ||
                                                  previousIndexLetter ==
                                                  (int)GeneralLetters.Hamza ||
                                                  previousIndexLetter ==
                                                  (int)GeneralLetters.AlefMaksoor ||
                                                  previousIndexLetter ==
                                                  (int)GeneralLetters.ZeroWidthNoJoiner ||
                                                  previousIndexLetter ==
                                                  (int)GeneralLetters.WawHamza ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.Alef ||
                                                  previousIndexLetter == (int)IsolatedLetters.Dal ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.Thal ||
                                                  previousIndexLetter == (int)IsolatedLetters.Ra2 ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.Zeen ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.PersianZe ||
                                                  previousIndexLetter == (int)IsolatedLetters.Waw ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.AlefMad ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.AlefHamza ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.Hamza ||
                                                  previousIndexLetter ==
                                                  (int)IsolatedLetters.AlefMaksoor;


            bool canThisLetterBeLeading = currentIndexLetter != ' ' &&
                                          currentIndexLetter != (int)GeneralLetters.Dal &&
                                          currentIndexLetter != (int)GeneralLetters.Thal &&
                                          currentIndexLetter != (int)GeneralLetters.Ra2 &&
                                          currentIndexLetter != (int)GeneralLetters.Zeen &&
                                          currentIndexLetter != (int)GeneralLetters.PersianZe &&
                                          currentIndexLetter != (int)GeneralLetters.Alef &&
                                          currentIndexLetter != (int)GeneralLetters.AlefHamza &&
                                          currentIndexLetter != (int)GeneralLetters.AlefMaksoor &&
                                          currentIndexLetter != (int)GeneralLetters.AlefMad &&
                                          currentIndexLetter != (int)GeneralLetters.WawHamza &&
                                          currentIndexLetter != (int)GeneralLetters.Waw &&
                                          currentIndexLetter !=
                                          (int)GeneralLetters.ZeroWidthNoJoiner &&
                                          currentIndexLetter != (int)GeneralLetters.Hamza;

            bool isNextLetterConnectable = index < letters.Length - 1 &&
                                           TextUtils.IsRTLCharacter(nextIndexLetter) &&
                                           nextIndexLetter != (int)GeneralLetters.Hamza &&
                                           nextIndexLetter !=
                                           (int)GeneralLetters.ZeroWidthNoJoiner;

            return isPreviousLetterNonConnectable &&
                   canThisLetterBeLeading &&
                   isNextLetterConnectable;
        }

        /// <summary>
        ///     Is the letter at provided index a finishing letter?
        /// </summary>
        /// <returns><see langword="true" /> if the letter is a finishing letter</returns>
        private static bool IsFinishingLetter(FastStringBuilder letters, int index)
        {
            char currentIndexLetter = letters.Get(index);

            char previousIndexLetter = default;
            if (index != 0)
                previousIndexLetter = letters.Get(index - 1);

            bool isPreviousLetterConnectable = index != 0 &&
                                               previousIndexLetter != ' ' &&
                                               previousIndexLetter != (int)GeneralLetters.Dal &&
                                               previousIndexLetter != (int)GeneralLetters.Thal &&
                                               previousIndexLetter != (int)GeneralLetters.Ra2 &&
                                               previousIndexLetter != (int)GeneralLetters.Zeen &&
                                               previousIndexLetter != (int)GeneralLetters.PersianZe &&
                                               previousIndexLetter != (int)GeneralLetters.Waw &&
                                               previousIndexLetter != (int)GeneralLetters.Alef &&
                                               previousIndexLetter != (int)GeneralLetters.AlefMad &&
                                               previousIndexLetter != (int)GeneralLetters.AlefHamza &&
                                               previousIndexLetter != (int)GeneralLetters.AlefMaksoor &&
                                               previousIndexLetter != (int)GeneralLetters.WawHamza &&
                                               previousIndexLetter != (int)GeneralLetters.Hamza &&
                                               previousIndexLetter != (int)GeneralLetters.ZeroWidthNoJoiner &&
                                               previousIndexLetter != (int)IsolatedLetters.Dal &&
                                               previousIndexLetter != (int)IsolatedLetters.Thal &&
                                               previousIndexLetter != (int)IsolatedLetters.Ra2 &&
                                               previousIndexLetter != (int)IsolatedLetters.Zeen &&
                                               previousIndexLetter != (int)IsolatedLetters.PersianZe &&
                                               previousIndexLetter != (int)IsolatedLetters.Waw &&
                                               previousIndexLetter != (int)IsolatedLetters.Alef &&
                                               previousIndexLetter != (int)IsolatedLetters.AlefMad &&
                                               previousIndexLetter != (int)IsolatedLetters.AlefHamza &&
                                               previousIndexLetter != (int)IsolatedLetters.AlefMaksoor &&
                                               previousIndexLetter != (int)IsolatedLetters.WawHamza &&
                                               previousIndexLetter != (int)IsolatedLetters.Hamza &&
                                               TextUtils.IsRTLCharacter(previousIndexLetter);


            bool canThisLetterBeFinishing = currentIndexLetter != ' ' &&
                                            currentIndexLetter != (int)GeneralLetters.ZeroWidthNoJoiner &&
                                            currentIndexLetter != (int)GeneralLetters.Hamza;

            return isPreviousLetterConnectable && canThisLetterBeFinishing;
        }

        /// <summary>
        ///     Is the letter at provided index a middle letter?
        /// </summary>
        /// <returns><see langword="true" /> if the letter is a middle letter</returns>
        private static bool IsMiddleLetter(FastStringBuilder letters, int index)
        {
            var currentIndexLetter = letters.Get(index);

            char previousIndexLetter = default;
            if (index != 0)
                previousIndexLetter = letters.Get(index - 1);

            char nextIndexLetter = default;
            if (index < letters.Length - 1)
                nextIndexLetter = letters.Get(index + 1);

            bool middleLetterCheck = index != 0 &&
                                     currentIndexLetter != (int)GeneralLetters.Alef &&
                                     currentIndexLetter != (int)GeneralLetters.Dal &&
                                     currentIndexLetter != (int)GeneralLetters.Thal &&
                                     currentIndexLetter != (int)GeneralLetters.Ra2 &&
                                     currentIndexLetter != (int)GeneralLetters.Zeen &&
                                     currentIndexLetter != (int)GeneralLetters.PersianZe &&
                                     currentIndexLetter != (int)GeneralLetters.Waw &&
                                     currentIndexLetter != (int)GeneralLetters.AlefMad &&
                                     currentIndexLetter != (int)GeneralLetters.AlefHamza &&
                                     currentIndexLetter != (int)GeneralLetters.AlefMaksoor &&
                                     currentIndexLetter != (int)GeneralLetters.WawHamza &&
                                     currentIndexLetter != (int)GeneralLetters.ZeroWidthNoJoiner &&
                                     currentIndexLetter != (int)GeneralLetters.Hamza;

            bool previousLetterCheck = index != 0 &&
                                       previousIndexLetter != (int)GeneralLetters.Alef &&
                                       previousIndexLetter != (int)GeneralLetters.Dal &&
                                       previousIndexLetter != (int)GeneralLetters.Thal &&
                                       previousIndexLetter != (int)GeneralLetters.Ra2 &&
                                       previousIndexLetter != (int)GeneralLetters.Zeen &&
                                       previousIndexLetter != (int)GeneralLetters.PersianZe &&
                                       previousIndexLetter != (int)GeneralLetters.Waw &&
                                       previousIndexLetter != (int)GeneralLetters.AlefMad &&
                                       previousIndexLetter != (int)GeneralLetters.AlefHamza &&
                                       previousIndexLetter != (int)GeneralLetters.AlefMaksoor &&
                                       previousIndexLetter != (int)GeneralLetters.WawHamza &&
                                       previousIndexLetter != (int)GeneralLetters.Hamza &&
                                       previousIndexLetter !=
                                       (int)GeneralLetters.ZeroWidthNoJoiner &&
                                       previousIndexLetter != (int)IsolatedLetters.Alef &&
                                       previousIndexLetter != (int)IsolatedLetters.Dal &&
                                       previousIndexLetter != (int)IsolatedLetters.Thal &&
                                       previousIndexLetter != (int)IsolatedLetters.Ra2 &&
                                       previousIndexLetter != (int)IsolatedLetters.Zeen &&
                                       previousIndexLetter != (int)IsolatedLetters.PersianZe &&
                                       previousIndexLetter != (int)IsolatedLetters.Waw &&
                                       previousIndexLetter != (int)IsolatedLetters.AlefMad &&
                                       previousIndexLetter != (int)IsolatedLetters.AlefHamza &&
                                       previousIndexLetter != (int)IsolatedLetters.AlefMaksoor &&
                                       previousIndexLetter != (int)IsolatedLetters.WawHamza &&
                                       previousIndexLetter != (int)IsolatedLetters.Hamza &&
                                       TextUtils.IsRTLCharacter(previousIndexLetter);

            bool nextLetterCheck = index < letters.Length - 1 &&
                                   TextUtils.IsRTLCharacter(nextIndexLetter) &&
                                   nextIndexLetter != (int)GeneralLetters.ZeroWidthNoJoiner &&
                                   nextIndexLetter != (int)GeneralLetters.Hamza &&
                                   nextIndexLetter != (int)IsolatedLetters.Hamza;

            return nextLetterCheck && previousLetterCheck && middleLetterCheck;
        }
    }
}