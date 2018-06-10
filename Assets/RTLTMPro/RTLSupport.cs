/*
 * This plugin is made possible by Arabic Support plugin created by: Abdulla Konash. Twitter: @konash
 * Original Arabic Support can be found here: https://github.com/Konash/arabic-support-unity
 */

using System.Collections.Generic;

namespace RTLTMPro
{
    public static class RTLSupport
    {
        public static string FixRTL(this string input, bool preserveNumbers = true, bool farsiNumbers = true, bool preserveTashkeel = false)
        {
            var tashkeelLocation = new List<TashkeelLocation>();
            string originString = RemoveTashkeel(input, tashkeelLocation);

            char[] lettersOrigin = originString.ToCharArray();
            char[] lettersFinal = originString.ToCharArray();


            for (int i = 0; i < lettersOrigin.Length; i++)
            {
                lettersOrigin[i] = (char) GlyphTable.Convert(lettersOrigin[i]);
            }

            for (int i = 0; i < lettersOrigin.Length; i++)
            {
                bool skipNext = false;

                // For special Lam Letter connections.
                if (lettersOrigin[i] == (char) IsolatedLetters.Lam)
                {
                    if (i < lettersOrigin.Length - 1)
                    {
                        skipNext = HandleSpecialLam(lettersOrigin, lettersFinal, i);
                    }
                }

                if (IsRTLCharacter(lettersOrigin[i]))
                {
                    if (IsMiddleLetter(lettersOrigin, i))
                        lettersFinal[i] = (char) (lettersOrigin[i] + 3);
                    else if (IsFinishingLetter(lettersOrigin, i))
                        lettersFinal[i] = (char) (lettersOrigin[i] + 1);
                    else if (IsLeadingLetter(lettersOrigin, i))
                        lettersFinal[i] = (char) (lettersOrigin[i] + 2);
                }

                if (skipNext)
                    i++;


                //chaning numbers to hindu
                if (!preserveNumbers)
                {
                    FixNumbers(lettersOrigin, lettersFinal, i, farsiNumbers);
                }
            }


            //Return the Tashkeel to their places.
            if (preserveTashkeel)
                lettersFinal = ReturnTashkeel(lettersFinal, tashkeelLocation);


            List<char> list = new List<char>();

            List<char> numberList = new List<char>();

            for (int i = lettersFinal.Length - 1; i >= 0; i--)
            {
                if (char.IsPunctuation(lettersFinal[i]) &&
                    i > 0 &&
                    i < lettersFinal.Length - 1 &&
                    (char.IsPunctuation(lettersFinal[i - 1]) || char.IsPunctuation(lettersFinal[i + 1])))
                {
                    if (lettersFinal[i] == '(')
                        list.Add(')');
                    else if (lettersFinal[i] == ')')
                        list.Add('(');
                    else if (lettersFinal[i] == '<')
                        list.Add('>');
                    else if (lettersFinal[i] == '>')
                        list.Add('<');
                    else if (lettersFinal[i] == '[')
                        list.Add(']');
                    else if (lettersFinal[i] == ']')
                        list.Add('[');
                    else if (lettersFinal[i] != 0xFFFF)
                        list.Add(lettersFinal[i]);
                }
                // For cases where english words and arabic are mixed. This allows for using arabic, english and numbers in one sentence.
                else if (lettersFinal[i] == ' ' &&
                         i > 0 &&
                         i < lettersFinal.Length - 1 &&
                         (char.IsLower(lettersFinal[i - 1]) || char.IsUpper(lettersFinal[i - 1]) || char.IsNumber(lettersFinal[i - 1])) &&
                         (char.IsLower(lettersFinal[i + 1]) || char.IsUpper(lettersFinal[i + 1]) || char.IsNumber(lettersFinal[i + 1])))

                {
                    numberList.Add(lettersFinal[i]);
                }

                else if (char.IsNumber(lettersFinal[i]) ||
                         char.IsLower(lettersFinal[i]) ||
                         char.IsUpper(lettersFinal[i]) ||
                         char.IsSymbol(lettersFinal[i]) ||
                         char.IsPunctuation(lettersFinal[i]))
                {
                    if (lettersFinal[i] == '(')
                        numberList.Add(')');
                    else if (lettersFinal[i] == ')')
                        numberList.Add('(');
                    else if (lettersFinal[i] == '<')
                        numberList.Add('>');
                    else if (lettersFinal[i] == '>')
                        numberList.Add('<');
                    else if (lettersFinal[i] == '[')
                        list.Add(']');
                    else if (lettersFinal[i] == ']')
                        list.Add('[');
                    else
                        numberList.Add(lettersFinal[i]);
                }
                else if (lettersFinal[i] >= (char) 0xD800 && lettersFinal[i] <= (char) 0xDBFF ||
                         lettersFinal[i] >= (char) 0xDC00 && lettersFinal[i] <= (char) 0xDFFF)
                {
                    numberList.Add(lettersFinal[i]);
                }
                else
                {
                    if (numberList.Count > 0)
                    {
                        for (int j = 0; j < numberList.Count; j++)
                            list.Add(numberList[numberList.Count - 1 - j]);
                        numberList.Clear();
                    }

                    if (lettersFinal[i] != 0xFFFF)
                        list.Add(lettersFinal[i]);
                }
            }

            if (numberList.Count > 0)
            {
                for (int j = 0; j < numberList.Count; j++)
                    list.Add(numberList[numberList.Count - 1 - j]);
                numberList.Clear();
            }

            // Moving letters from a list to an array.
            lettersFinal = new char[list.Count];
            for (int i = 0; i < lettersFinal.Length; i++)
                lettersFinal[i] = list[i];


            input = new string(lettersFinal);
            return input;
        }

        private static bool HandleSpecialLam(IList<char> lettersOrigin, IList<char> lettersFinal, int i)
        {
            bool skip = false;

            if (lettersOrigin[i + 1] == (char) IsolatedLetters.AlefMaksoor)
            {
                lettersOrigin[i] = (char) 0xFEF7;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }
            else if (lettersOrigin[i + 1] == (char) IsolatedLetters.Alef)
            {
                lettersOrigin[i] = (char) 0xFEF9;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }
            else if (lettersOrigin[i + 1] == (char) IsolatedLetters.AlefHamza)
            {
                lettersOrigin[i] = (char) 0xFEF5;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }
            else if (lettersOrigin[i + 1] == (char) IsolatedLetters.AlefMad)
            {
                lettersOrigin[i] = (char) 0xFEF3;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }

            return skip;
        }

        private static void FixNumbers(IList<char> lettersOrigin, IList<char> lettersFinal, int i, bool farsiNumbers)
        {
            if (lettersOrigin[i] == (char) 0x0030)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Zero : (char) HinduNumbers.Zero;
            else if (lettersOrigin[i] == (char) 0x0031)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.One : (char) HinduNumbers.One;
            else if (lettersOrigin[i] == (char) 0x0032)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Two : (char) HinduNumbers.Two;
            else if (lettersOrigin[i] == (char) 0x0033)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Three : (char) HinduNumbers.Three;
            else if (lettersOrigin[i] == (char) 0x0034)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Four : (char) HinduNumbers.Four;
            else if (lettersOrigin[i] == (char) 0x0035)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Five : (char) HinduNumbers.Five;
            else if (lettersOrigin[i] == (char) 0x0036)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Six : (char) HinduNumbers.Six;
            else if (lettersOrigin[i] == (char) 0x0037)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Seven : (char) HinduNumbers.Seven;
            else if (lettersOrigin[i] == (char) 0x0038)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Eight : (char) HinduNumbers.Eight;
            else if (lettersOrigin[i] == (char) 0x0039)
                lettersFinal[i] = farsiNumbers ? (char) FarsiNumbers.Nine : (char) HinduNumbers.Nine;
        }

        private static string RemoveTashkeel(string str, ICollection<TashkeelLocation> tashkeelLocation)
        {
            char[] letters = str.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == (char) 0x064B)
                {
                    // Tanween Fatha
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x064B, i));
                }
                else if (letters[i] == (char) 0x064C)
                {
                    // Tanween Damma
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x064C, i));
                }
                else if (letters[i] == (char) 0x064D)
                {
                    // Tanween Kasra
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x064D, i));
                }
                else if (letters[i] == (char) 0x064E)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x064E, i));
                }
                else if (letters[i] == (char) 0x064F)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x064F, i));
                }
                else if (letters[i] == (char) 0x0650)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x0650, i));
                }
                else if (letters[i] == (char) 0x0651)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x0651, i));
                }
                else if (letters[i] == (char) 0x0652)
                {
                    // SUKUN
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x0652, i));
                }
                else if (letters[i] == (char) 0x0653)
                {
                    // MADDAH ABOVE
                    tashkeelLocation.Add(new TashkeelLocation((char) 0x0653, i));
                }
            }

            string[] split = str.Split((char) 0x064B, (char) 0x064C, (char) 0x064D, (char) 0x064E, (char) 0x064F, (char) 0x0650, (char) 0x0651, (char) 0x0652, (char) 0x0653, (char) 0xFC60,
                (char) 0xFC61, (char) 0xFC62);
            str = "";

            foreach (string s in split)
            {
                str += s;
            }

            return str;
        }

        private static char[] ReturnTashkeel(ICollection<char> letters, ICollection<TashkeelLocation> tashkeelLocation)
        {
            char[] lettersWithTashkeel = new char[letters.Count + tashkeelLocation.Count];

            int letterWithTashkeelTracker = 0;
            foreach (var t in letters)
            {
                lettersWithTashkeel[letterWithTashkeelTracker] = t;
                letterWithTashkeelTracker++;
                foreach (TashkeelLocation hLocation in tashkeelLocation)
                {
                    if (hLocation.Position == letterWithTashkeelTracker)
                    {
                        lettersWithTashkeel[letterWithTashkeelTracker] = hLocation.Tashkeel;
                        letterWithTashkeelTracker++;
                    }
                }
            }

            return lettersWithTashkeel;
        }
        
        private static bool IsRTLCharacter(char ch)
        {
            // If it's not letter, it's not RTL letter
            if (char.IsLetter(ch) == false)
                return false;

            // Skip English letters
            if (char.IsUpper(ch) || char.IsLower(ch))
                return false;

            return true;
        }

        /// <summary>
        ///     Checks if the letter at index value is a leading character or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a leading character, else, returns false</returns>
        private static bool IsLeadingLetter(char[] letters, int index)
        {
            bool previousLetterCheck = index == 0 ||
                                       IsRTLCharacter(letters[index - 1]) == false ||
                                       letters[index - 1] == (int) IsolatedLetters.Alef ||
                                       letters[index - 1] == (int) IsolatedLetters.Dal ||
                                       letters[index - 1] == (int) IsolatedLetters.Thal ||
                                       letters[index - 1] == (int) IsolatedLetters.Ra2 ||
                                       letters[index - 1] == (int) IsolatedLetters.Zeen ||
                                       letters[index - 1] == (int) IsolatedLetters.PersianZe ||
                                       letters[index - 1] == (int) IsolatedLetters.Waw ||
                                       letters[index - 1] == (int) IsolatedLetters.AlefMad ||
                                       letters[index - 1] == (int) IsolatedLetters.AlefHamza ||
                                       letters[index - 1] == (int) IsolatedLetters.Hamza ||
                                       letters[index - 1] == (int) IsolatedLetters.AlefMaksoor ||
                                       letters[index - 1] == (int) IsolatedLetters.WawHamza;

            bool leadingLetterCheck = letters[index] != ' ' &&
                                      letters[index] != (int) IsolatedLetters.Dal &&
                                      letters[index] != (int) IsolatedLetters.Thal &&
                                      letters[index] != (int) IsolatedLetters.Ra2 &&
                                      letters[index] != (int) IsolatedLetters.Zeen &&
                                      letters[index] != (int) IsolatedLetters.PersianZe &&
                                      letters[index] != (int) IsolatedLetters.Alef &&
                                      letters[index] != (int) IsolatedLetters.AlefHamza &&
                                      letters[index] != (int) IsolatedLetters.AlefMaksoor &&
                                      letters[index] != (int) IsolatedLetters.AlefMad &&
                                      letters[index] != (int) IsolatedLetters.WawHamza &&
                                      letters[index] != (int) IsolatedLetters.Waw &&
                                      letters[index] != (int) IsolatedLetters.Hamza;

            bool nextLetterCheck = index < letters.Length - 1 &&
                                   IsRTLCharacter(letters[index + 1]) && 
                                   letters[index + 1] != (int) IsolatedLetters.Hamza;

            return previousLetterCheck && leadingLetterCheck && nextLetterCheck;
        }

        /// <summary>
        ///     Checks if the letter at index value is a finishing character or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a finishing character, else, returns false</returns>
        private static bool IsFinishingLetter(IList<char> letters, int index)
        {
            bool previousLetterCheck = index != 0 &&
                                       letters[index - 1] != ' ' &&
                                       letters[index - 1] != (int) IsolatedLetters.Dal &&
                                       letters[index - 1] != (int) IsolatedLetters.Thal &&
                                       letters[index - 1] != (int) IsolatedLetters.Ra2 &&
                                       letters[index - 1] != (int) IsolatedLetters.Zeen &&
                                       letters[index - 1] != (int) IsolatedLetters.PersianZe &&
                                       letters[index - 1] != (int) IsolatedLetters.Waw &&
                                       letters[index - 1] != (int) IsolatedLetters.Alef &&
                                       letters[index - 1] != (int) IsolatedLetters.AlefMad &&
                                       letters[index - 1] != (int) IsolatedLetters.AlefHamza &&
                                       letters[index - 1] != (int) IsolatedLetters.AlefMaksoor &&
                                       letters[index - 1] != (int) IsolatedLetters.WawHamza &&
                                       letters[index - 1] != (int) IsolatedLetters.Hamza &&
                                       IsRTLCharacter(letters[index - 1]);


            bool finishingLetterCheck = letters[index] != ' ' && letters[index] != (int) IsolatedLetters.Hamza;


            return previousLetterCheck && finishingLetterCheck;
        }

        /// <summary>
        ///     Checks if the letter at index value is a middle character or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a middle character, else, returns false</returns>
        private static bool IsMiddleLetter(IList<char> letters, int index)
        {
            bool middleLetterCheck = index != 0 &&
                                     letters[index] != (int) IsolatedLetters.Alef &&
                                     letters[index] != (int) IsolatedLetters.Dal &&
                                     letters[index] != (int) IsolatedLetters.Thal &&
                                     letters[index] != (int) IsolatedLetters.Ra2 &&
                                     letters[index] != (int) IsolatedLetters.Zeen &&
                                     letters[index] != (int) IsolatedLetters.PersianZe &&
                                     letters[index] != (int) IsolatedLetters.Waw &&
                                     letters[index] != (int) IsolatedLetters.AlefMad &&
                                     letters[index] != (int) IsolatedLetters.AlefHamza &&
                                     letters[index] != (int) IsolatedLetters.AlefMaksoor &&
                                     letters[index] != (int) IsolatedLetters.WawHamza &&
                                     letters[index] != (int) IsolatedLetters.Hamza;

            bool previousLetterCheck = index != 0 &&
                                       letters[index - 1] != (int) IsolatedLetters.Alef &&
                                       letters[index - 1] != (int) IsolatedLetters.Dal &&
                                       letters[index - 1] != (int) IsolatedLetters.Thal &&
                                       letters[index - 1] != (int) IsolatedLetters.Ra2 &&
                                       letters[index - 1] != (int) IsolatedLetters.Zeen &&
                                       letters[index - 1] != (int) IsolatedLetters.PersianZe &&
                                       letters[index - 1] != (int) IsolatedLetters.Waw &&
                                       letters[index - 1] != (int) IsolatedLetters.AlefMad &&
                                       letters[index - 1] != (int) IsolatedLetters.AlefHamza &&
                                       letters[index - 1] != (int) IsolatedLetters.AlefMaksoor &&
                                       letters[index - 1] != (int) IsolatedLetters.WawHamza &&
                                       letters[index - 1] != (int) IsolatedLetters.Hamza &&
                                       IsRTLCharacter(letters[index - 1]);

            bool nextLetterCheck = index < letters.Count - 1 &&
                                   IsRTLCharacter(letters[index + 1]) &&
                                   letters[index + 1] != (int) IsolatedLetters.Hamza;

            return nextLetterCheck && previousLetterCheck && middleLetterCheck;
        }
    }
}