/*
 * This plugin is made possible by Arabic Support plugin created by: Abdulla Konash. Twitter: @konash
 * Original Arabic Support can be found here: https://github.com/Konash/arabic-support-unity
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace RTLTMPro
{
    public class RTLSupport
    {
        // Because we are initializing these properties in constructor, we cannot make them virtual
        public bool PreserveNumbers { get; set; }
        public bool PreserveTashkeel { get; set; }
        public bool FixTags { get; set; }
        public bool Farsi { get; set; }

        protected readonly ICollection<TashkeelLocation> TashkeelLocation;

        public RTLSupport()
        {
            PreserveNumbers = false;
            PreserveTashkeel = false;
            Farsi = true;
            FixTags = false;

            TashkeelLocation = new List<TashkeelLocation>();
        }

        public string FixRTL(string input)
        {
            List<char> finalLetters = new List<char>();
            TashkeelLocation.Clear();

            char[] letters = PrepareInput(input);
            char[] fixedLetters = FixGlyphs(letters);
            FixLigature(fixedLetters, finalLetters);

            if (FixTags)
                FixTextTags(finalLetters);

            return new string(finalLetters.ToArray());
        }

        protected virtual void FixTextTags(List<char> finalLetters)
        {
            int openIndex = -1;
            List<char> tag = new List<char>();
            for (int i = 0; i < finalLetters.Count; i++)
            {
                if (finalLetters[i] == '<')
                {
                    openIndex = i;
                }

                if (finalLetters[i] == '>')
                {
                    if (openIndex == -1)
                        return;

                    tag.Clear();
                    for (int j = openIndex; j <= i; j++)
                    {
                        tag.Add(finalLetters[j]);
                    }

                    tag.Reverse();
                    finalLetters.RemoveRange(openIndex, tag.Count);
                    finalLetters.InsertRange(openIndex, tag);

                    openIndex = -1;
                }
            }
        }

        protected virtual char[] PrepareInput(string input)
        {
            string originString = RemoveTashkeel(input);
            char[] letters = originString.ToCharArray();
            for (int i = 0; i < letters.Length; i++)
            {
                if (Farsi && letters[i] == (int) GeneralLetters.Ya)
                    letters[i] = (char) GeneralLetters.PersianYa;
                else if (Farsi == false && letters[i] == (int) GeneralLetters.PersianYa)
                    letters[i] = (char) GeneralLetters.Ya;

                letters[i] = (char) GlyphTable.Convert(letters[i]);
            }

            return letters;
        }

        protected virtual string RemoveTashkeel(string str)
        {
            char[] letters = str.ToCharArray();

            if (PreserveTashkeel)
            {
                for (int i = 0; i < letters.Length; i++)
                {
                    if (letters[i] == (char) 0x064B)
                    {
                        // Tanween Fatha
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x064B, i));
                    }
                    else if (letters[i] == (char) 0x064C)
                    {
                        // Tanween Damma
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x064C, i));
                    }
                    else if (letters[i] == (char) 0x064D)
                    {
                        // Tanween Kasra
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x064D, i));
                    }
                    else if (letters[i] == (char) 0x064E)
                    {
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x064E, i));
                    }
                    else if (letters[i] == (char) 0x064F)
                    {
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x064F, i));
                    }
                    else if (letters[i] == (char) 0x0650)
                    {
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x0650, i));
                    }
                    else if (letters[i] == (char) 0x0651)
                    {
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x0651, i));
                    }
                    else if (letters[i] == (char) 0x0652)
                    {
                        // SUKUN
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x0652, i));
                    }
                    else if (letters[i] == (char) 0x0653)
                    {
                        // MADDAH ABOVE
                        TashkeelLocation.Add(new TashkeelLocation((char) 0x0653, i));
                    }
                }
            }

            string[] split = str.Split((char) 0x064B, (char) 0x064C, (char) 0x064D, (char) 0x064E, (char) 0x064F, (char) 0x0650, (char) 0x0651, (char) 0x0652, (char) 0x0653, (char) 0xFC60,
                (char) 0xFC61, (char) 0xFC62);

            return split.Aggregate("", (current, s) => current + s);
        }

        protected virtual char[] FixGlyphs(char[] letters)
        {
            char[] lettersFinal = new char[letters.Length];
            Array.Copy(letters, lettersFinal, letters.Length);
            for (int i = 0; i < letters.Length; i++)
            {
                bool skipNext = false;

                // For special Lam Letter connections.
                if (letters[i] == (char) IsolatedLetters.Lam)
                {
                    if (i < letters.Length - 1)
                    {
                        skipNext = HandleSpecialLam(letters, lettersFinal, i);
                    }
                }

                if (IsRTLCharacter(letters[i]))
                {
                    if (IsMiddleLetter(letters, i))
                        lettersFinal[i] = (char) (letters[i] + 3);
                    else if (IsFinishingLetter(letters, i))
                        lettersFinal[i] = (char) (letters[i] + 1);
                    else if (IsLeadingLetter(letters, i))
                        lettersFinal[i] = (char) (letters[i] + 2);
                }

                if (skipNext)
                    i++;

                if (!PreserveNumbers && char.IsDigit(letters[i]))
                {
                    lettersFinal[i] = FixNumbers(letters[i]);
                }
            }

            //Restore tashkeel to their places.
            if (PreserveTashkeel)
                lettersFinal = RestoreTashkeel(lettersFinal);
            return lettersFinal;
        }

        protected virtual char FixNumbers(char num)
        {
            switch (num)
            {
                case (char) 0x0030:
                    return Farsi ? (char) FarsiNumbers.Zero : (char) HinduNumbers.Zero;
                case (char) 0x0031:
                    return Farsi ? (char) FarsiNumbers.One : (char) HinduNumbers.One;
                case (char) 0x0032:
                    return Farsi ? (char) FarsiNumbers.Two : (char) HinduNumbers.Two;
                case (char) 0x0033:
                    return Farsi ? (char) FarsiNumbers.Three : (char) HinduNumbers.Three;
                case (char) 0x0034:
                    return Farsi ? (char) FarsiNumbers.Four : (char) HinduNumbers.Four;
                case (char) 0x0035:
                    return Farsi ? (char) FarsiNumbers.Five : (char) HinduNumbers.Five;
                case (char) 0x0036:
                    return Farsi ? (char) FarsiNumbers.Six : (char) HinduNumbers.Six;
                case (char) 0x0037:
                    return Farsi ? (char) FarsiNumbers.Seven : (char) HinduNumbers.Seven;
                case (char) 0x0038:
                    return Farsi ? (char) FarsiNumbers.Eight : (char) HinduNumbers.Eight;
                case (char) 0x0039:
                    return Farsi ? (char) FarsiNumbers.Nine : (char) HinduNumbers.Nine;
            }

            return num;
        }

        protected virtual void FixLigature(IList<char> fixedLetters, ICollection<char> finalLetters)
        {
            List<char> preserveOrder = new List<char>();
            for (int i = fixedLetters.Count - 1; i >= 0; i--)
            {
                if (char.IsPunctuation(fixedLetters[i]) || char.IsSymbol(fixedLetters[i]))
                {
                    if (i > 0 && i < fixedLetters.Count - 1)
                    {
                        if (IsRTLCharacter(fixedLetters[i - 1]) && IsRTLCharacter(fixedLetters[i + 1]) ||
                            char.IsWhiteSpace(fixedLetters[i + 1]) && (fixedLetters[i] == '.' || fixedLetters[i] == '،' || fixedLetters[i] == '؛') ||
                            char.IsWhiteSpace(fixedLetters[i - 1]) && IsRTLCharacter(fixedLetters[i + 1]) ||
                            IsRTLCharacter(fixedLetters[i - 1]) && char.IsWhiteSpace(fixedLetters[i + 1]))
                        {
                            finalLetters.Add(fixedLetters[i]);
                        }
                        else
                        {
                            preserveOrder.Add(fixedLetters[i]);
                        }
                    }
                    else if (i == 0)
                    {
                        finalLetters.Add(fixedLetters[i]);
                    }
                    else if (i == fixedLetters.Count - 1)
                    {
                        // If the punctuation is at the ending of the text, if the text is not RTL, preserve order
                        if (IsRTLInput(fixedLetters))
                            finalLetters.Add(fixedLetters[i]);
                        else
                            preserveOrder.Add(fixedLetters[i]);
                    }

                    continue;
                }

                // For cases where english words and arabic are mixed. This allows for using arabic, english and numbers in one sentence.
                if (fixedLetters[i] == ' ' &&
                    i > 0 &&
                    i < fixedLetters.Count - 1 &&
                    (char.IsLower(fixedLetters[i - 1]) || char.IsUpper(fixedLetters[i - 1]) || char.IsNumber(fixedLetters[i - 1])) &&
                    (char.IsLower(fixedLetters[i + 1]) || char.IsUpper(fixedLetters[i + 1]) || char.IsNumber(fixedLetters[i + 1])))

                {
                    preserveOrder.Add(fixedLetters[i]);
                }

                else if (char.IsNumber(fixedLetters[i]) ||
                         char.IsLower(fixedLetters[i]) ||
                         char.IsUpper(fixedLetters[i]))
                {
                    preserveOrder.Add(fixedLetters[i]);
                }
                else if (fixedLetters[i] >= (char) 0xD800 && fixedLetters[i] <= (char) 0xDBFF ||
                         fixedLetters[i] >= (char) 0xDC00 && fixedLetters[i] <= (char) 0xDFFF)
                {
                    preserveOrder.Add(fixedLetters[i]);
                }
                else
                {
                    if (preserveOrder.Count > 0)
                    {
                        for (int j = 0; j < preserveOrder.Count; j++)
                            finalLetters.Add(preserveOrder[preserveOrder.Count - 1 - j]);
                        preserveOrder.Clear();
                    }

                    if (fixedLetters[i] != 0xFFFF)
                        finalLetters.Add(fixedLetters[i]);
                }
            }

            if (preserveOrder.Count > 0)
            {
                for (int j = 0; j < preserveOrder.Count; j++)
                    finalLetters.Add(preserveOrder[preserveOrder.Count - 1 - j]);
                preserveOrder.Clear();
            }
        }

        protected virtual char[] RestoreTashkeel(ICollection<char> letters)
        {
            char[] lettersWithTashkeel = new char[letters.Count + TashkeelLocation.Count];

            int letterWithTashkeelTracker = 0;
            foreach (var t in letters)
            {
                lettersWithTashkeel[letterWithTashkeelTracker] = t;
                letterWithTashkeelTracker++;
                foreach (TashkeelLocation hLocation in TashkeelLocation)
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

        public static bool IsRTLCharacter(char ch)
        {
            // If it's not letter, it's not RTL letter
            if (char.IsLetter(ch) == false)
                return false;

            // Skip English letters
            if (char.IsUpper(ch) || char.IsLower(ch))
                return false;

            return true;
        }

        public static bool IsRTLInput(string input)
        {
            char[] chars = input.ToCharArray();
            return IsRTLInput(chars);
        }

        public static bool IsRTLInput(IEnumerable<char> chars)
        {
            return (from character in chars
                where char.IsLetter(character)
                select IsRTLCharacter(character)).FirstOrDefault();
        }

        protected static bool HandleSpecialLam(IList<char> letters, IList<char> lettersFinal, int i)
        {
            bool skip = false;

            if (letters[i + 1] == (char) IsolatedLetters.AlefMaksoor)
            {
                letters[i] = (char) 0xFEF7;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }
            else if (letters[i + 1] == (char) IsolatedLetters.Alef)
            {
                letters[i] = (char) 0xFEF9;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }
            else if (letters[i + 1] == (char) IsolatedLetters.AlefHamza)
            {
                letters[i] = (char) 0xFEF5;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }
            else if (letters[i + 1] == (char) IsolatedLetters.AlefMad)
            {
                letters[i] = (char) 0xFEF3;
                lettersFinal[i + 1] = (char) 0xFFFF;
                skip = true;
            }

            return skip;
        }

        protected static bool IsLeadingLetter(IList<char> letters, int index)
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

            bool nextLetterCheck = index < letters.Count - 1 &&
                                   IsRTLCharacter(letters[index + 1]) &&
                                   letters[index + 1] != (int) IsolatedLetters.Hamza;

            return previousLetterCheck && leadingLetterCheck && nextLetterCheck;
        }

        protected static bool IsFinishingLetter(IList<char> letters, int index)
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

        protected static bool IsMiddleLetter(IList<char> letters, int index)
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