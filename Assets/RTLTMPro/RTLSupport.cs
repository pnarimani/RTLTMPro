/*
 * This plugin is made possible by Arabic Support plugin created by: Abdulla Konash. Twitter: @konash
 * Original Arabic Support can be found here: https://github.com/Konash/arabic-support-unity
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RTLTMPro
{
    public class RTLSupport
    {
        // Because we are initializing these properties in constructor, we cannot make them virtual
        public bool PreserveNumbers { get; set; }
        public bool FixTags { get; set; }
        public bool Farsi { get; set; }

        protected readonly ICollection<TashkeelLocation> TashkeelLocation;

        public RTLSupport()
        {
            PreserveNumbers = false;
            Farsi = true;
            FixTags = false;

            TashkeelLocation = new List<TashkeelLocation>();
        }

        public virtual string FixRTL(string input)
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

                if (openIndex > -1)
                {
                    if (IsRTLCharacter(finalLetters[i]) || finalLetters[i] == ' ')
                    {
                        // cancel operation
                        openIndex = -1;
                        continue;
                    }
                }

                if (finalLetters[i] == '>')
                {
                    if (openIndex == -1)
                        continue;
                    
                    tag.Clear();
                    for (int j = openIndex; j <= i; j++)
                    {
                        if (Farsi)
                        {
                            switch (finalLetters[j])
                            {
                                case (char)FarsiNumbers.Zero:
                                    finalLetters[j] = (char)EnglishNumbers.Zero;
                                    break;
                                case (char)FarsiNumbers.One:
                                    finalLetters[j] = (char)EnglishNumbers.One;
                                    break;
                                case (char)FarsiNumbers.Two:
                                    finalLetters[j] = (char)EnglishNumbers.Two;
                                    break;
                                case (char)FarsiNumbers.Three:
                                    finalLetters[j] = (char)EnglishNumbers.Three;
                                    break;
                                case (char)FarsiNumbers.Four:
                                    finalLetters[j] = (char)EnglishNumbers.Four;
                                    break;
                                case (char)FarsiNumbers.Five:
                                    finalLetters[j] = (char)EnglishNumbers.Five;
                                    break;
                                case (char)FarsiNumbers.Six:
                                    finalLetters[j] = (char)EnglishNumbers.Six;
                                    break;
                                case (char)FarsiNumbers.Seven:
                                    finalLetters[j] = (char)EnglishNumbers.Seven;
                                    break;
                                case (char)FarsiNumbers.Eight:
                                    finalLetters[j] = (char)EnglishNumbers.Eight;
                                    break;
                                case (char)FarsiNumbers.Nine:
                                    finalLetters[j] = (char)EnglishNumbers.Nine;
                                    break;
                            }
                        }
                        else
                        {
                            switch (finalLetters[j])
                            {
                                case (char)HinduNumbers.Zero:
                                    finalLetters[j] = (char)EnglishNumbers.Zero;
                                    break;
                                case (char)HinduNumbers.One:
                                    finalLetters[j] = (char)EnglishNumbers.One;
                                    break;
                                case (char)HinduNumbers.Two:
                                    finalLetters[j] = (char)EnglishNumbers.Two;
                                    break;
                                case (char)HinduNumbers.Three:
                                    finalLetters[j] = (char)EnglishNumbers.Three;
                                    break;
                                case (char)HinduNumbers.Four:
                                    finalLetters[j] = (char)EnglishNumbers.Four;
                                    break;
                                case (char)HinduNumbers.Five:
                                    finalLetters[j] = (char)EnglishNumbers.Five;
                                    break;
                                case (char)HinduNumbers.Six:
                                    finalLetters[j] = (char)EnglishNumbers.Six;
                                    break;
                                case (char)HinduNumbers.Seven:
                                    finalLetters[j] = (char)EnglishNumbers.Seven;
                                    break;
                                case (char)HinduNumbers.Eight:
                                    finalLetters[j] = (char)EnglishNumbers.Eight;
                                    break;
                                case (char)HinduNumbers.Nine:
                                    finalLetters[j] = (char)EnglishNumbers.Nine;
                                    break;
                            }
                        }

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
                if (Farsi && letters[i] == (int)GeneralLetters.Ya)
                {
                    letters[i] = (char)GeneralLetters.PersianYa;
                }
                else if (Farsi == false && letters[i] == (int)GeneralLetters.PersianYa)
                {
                    letters[i] = (char)GeneralLetters.Ya;
                }

                letters[i] = (char)GlyphTable.Convert(letters[i]);
            }

            return letters;
        }

        protected virtual string RemoveTashkeel(string str)
        {
            char[] letters = str.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == (char)0x064B)
                {
                    // Tanween Fatha
                    TashkeelLocation.Add(new TashkeelLocation((char)0x064B, i));
                }
                else if (letters[i] == (char)0x064C)
                {
                    // Tanween Damma
                    TashkeelLocation.Add(new TashkeelLocation((char)0x064C, i));
                }
                else if (letters[i] == (char)0x064D)
                {
                    // Tanween Kasra
                    TashkeelLocation.Add(new TashkeelLocation((char)0x064D, i));
                }
                else if (letters[i] == (char)0x064E)
                {
                    TashkeelLocation.Add(new TashkeelLocation((char)0x064E, i));
                }
                else if (letters[i] == (char)0x064F)
                {
                    TashkeelLocation.Add(new TashkeelLocation((char)0x064F, i));
                }
                else if (letters[i] == (char)0x0650)
                {
                    TashkeelLocation.Add(new TashkeelLocation((char)0x0650, i));
                }
                else if (letters[i] == (char)0x0651)
                {
                    TashkeelLocation.Add(new TashkeelLocation((char)0x0651, i));
                }
                else if (letters[i] == (char)0x0652)
                {
                    // SUKUN
                    TashkeelLocation.Add(new TashkeelLocation((char)0x0652, i));
                }
                else if (letters[i] == (char)0x0653)
                {
                    // MADDAH ABOVE
                    TashkeelLocation.Add(new TashkeelLocation((char)0x0653, i));
                }
            }

            string[] split = str.Split((char)0x064B, (char)0x064C, (char)0x064D, (char)0x064E, (char)0x064F, (char)0x0650, (char)0x0651, (char)0x0652, (char)0x0653, (char)0xFC60,
                (char)0xFC61, (char)0xFC62);

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
                if (letters[i] == (char)IsolatedLetters.Lam)
                {
                    if (i < letters.Length - 1)
                    {
                        skipNext = HandleSpecialLam(letters, lettersFinal, i);
                    }
                }

                if (IsRTLCharacter(letters[i]))
                {
                    if (IsMiddleLetter(letters, i))
                        lettersFinal[i] = (char)(letters[i] + 3);
                    else if (IsFinishingLetter(letters, i))
                        lettersFinal[i] = (char)(letters[i] + 1);
                    else if (IsLeadingLetter(letters, i))
                        lettersFinal[i] = (char)(letters[i] + 2);
                }

                if (skipNext)
                {
                    i++;
                    continue;
                }

                if (!PreserveNumbers && char.IsDigit(letters[i]))
                {
                    lettersFinal[i] = FixNumbers(letters[i]);
                }
            }

            //Restore tashkeel to their places.
            lettersFinal = RestoreTashkeel(lettersFinal);
            return lettersFinal;
        }

        protected virtual char FixNumbers(char num)
        {
            switch (num)
            {
                case (char)EnglishNumbers.Zero:
                    return Farsi ? (char)FarsiNumbers.Zero : (char)HinduNumbers.Zero;
                case (char)EnglishNumbers.One:
                    return Farsi ? (char)FarsiNumbers.One : (char)HinduNumbers.One;
                case (char)EnglishNumbers.Two:
                    return Farsi ? (char)FarsiNumbers.Two : (char)HinduNumbers.Two;
                case (char)EnglishNumbers.Three:
                    return Farsi ? (char)FarsiNumbers.Three : (char)HinduNumbers.Three;
                case (char)EnglishNumbers.Four:
                    return Farsi ? (char)FarsiNumbers.Four : (char)HinduNumbers.Four;
                case (char)EnglishNumbers.Five:
                    return Farsi ? (char)FarsiNumbers.Five : (char)HinduNumbers.Five;
                case (char)EnglishNumbers.Six:
                    return Farsi ? (char)FarsiNumbers.Six : (char)HinduNumbers.Six;
                case (char)EnglishNumbers.Seven:
                    return Farsi ? (char)FarsiNumbers.Seven : (char)HinduNumbers.Seven;
                case (char)EnglishNumbers.Eight:
                    return Farsi ? (char)FarsiNumbers.Eight : (char)HinduNumbers.Eight;
                case (char)EnglishNumbers.Nine:
                    return Farsi ? (char)FarsiNumbers.Nine : (char)HinduNumbers.Nine;
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
                    if (FixTags)
                    {
                        if (fixedLetters[i] == '>')
                        {
                            if (preserveOrder.Count > 0)
                            {
                                for (int j = 0; j < preserveOrder.Count; j++)
                                    finalLetters.Add(preserveOrder[preserveOrder.Count - 1 - j]);
                                preserveOrder.Clear();
                            }
                        }
                    }

                    if (i > 0 && i < fixedLetters.Count - 1)
                    {
                        // NOTE: Array is reversed. i + 1 is behind and i - 1 is ahead
                        bool isAfterRTLCharacter = IsRTLCharacter(fixedLetters[i + 1]);
                        bool isBeforeRTLCharacter = IsRTLCharacter(fixedLetters[i - 1]);
                        bool isBeforeWhiteSpace = char.IsWhiteSpace(fixedLetters[i - 1]);
                        bool isAfterWhiteSpace = char.IsWhiteSpace(fixedLetters[i + 1]);
                        bool isSpecialPunctuation = fixedLetters[i] == '.' || fixedLetters[i] == '،' || fixedLetters[i] == '؛';

                        if (isBeforeRTLCharacter && isAfterRTLCharacter ||
                            isAfterWhiteSpace && isSpecialPunctuation ||
                            isBeforeWhiteSpace && isAfterRTLCharacter ||
                            isBeforeRTLCharacter && isAfterWhiteSpace)
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

                    if (FixTags)
                    {
                        if (fixedLetters[i] == '<')
                        {
                            if (preserveOrder.Count > 0)
                            {
                                for (int j = 0; j < preserveOrder.Count; j++)
                                    finalLetters.Add(preserveOrder[preserveOrder.Count - 1 - j]);
                                preserveOrder.Clear();
                            }
                        }
                    }
                    continue;
                }


                // For cases where english words and arabic are mixed. This allows for using arabic, english and numbers in one sentence.
                // If the space is between numbers,symbols or English words, keep the order
                if (fixedLetters[i] == ' ' &&
                    i > 0 &&
                    i < fixedLetters.Count - 1 &&
                    (char.IsLower(fixedLetters[i - 1]) || char.IsUpper(fixedLetters[i - 1]) || char.IsNumber(fixedLetters[i - 1]) || char.IsSymbol(fixedLetters[i - 1])) &&
                    (char.IsLower(fixedLetters[i + 1]) || char.IsUpper(fixedLetters[i + 1]) || char.IsNumber(fixedLetters[i + 1]) || char.IsSymbol(fixedLetters[i + 1])))

                {
                    preserveOrder.Add(fixedLetters[i]);
                }

                else if (char.IsNumber(fixedLetters[i]) ||
                         char.IsLower(fixedLetters[i]) ||
                         char.IsUpper(fixedLetters[i]))
                {
                    preserveOrder.Add(fixedLetters[i]);
                }
                else if (fixedLetters[i] >= (char)0xD800 && fixedLetters[i] <= (char)0xDBFF ||
                         fixedLetters[i] >= (char)0xDC00 && fixedLetters[i] <= (char)0xDFFF)
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

        public virtual bool IsRTLCharacter(char ch)
        {
            if (ch >= (char)IsolatedLetters.Hamza && ch <= (char)IsolatedLetters.Hamza + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Alef && ch <= (char)IsolatedLetters.Alef + 3)
                return true;

            if (ch >= (char)IsolatedLetters.AlefHamza && ch <= (char)IsolatedLetters.AlefHamza + 3)
                return true;

            if (ch >= (char)IsolatedLetters.WawHamza && ch <= (char)IsolatedLetters.WawHamza + 3)
                return true;

            if (ch >= (char)IsolatedLetters.AlefMaksoor && ch <= (char)IsolatedLetters.AlefMaksoor + 3)
                return true;

            if (ch >= (char)IsolatedLetters.AlefMaksora && ch <= (char)IsolatedLetters.AlefMaksora + 3)
                return true;

            if (ch >= (char)IsolatedLetters.HamzaNabera && ch <= (char)IsolatedLetters.HamzaNabera + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Ba && ch <= (char)IsolatedLetters.Ba + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Ta && ch <= (char)IsolatedLetters.Ta + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Tha2 && ch <= (char)IsolatedLetters.Tha2 + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Jeem && ch <= (char)IsolatedLetters.Jeem + 3)
                return true;

            if (ch >= (char)IsolatedLetters.H7aa && ch <= (char)IsolatedLetters.H7aa + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Khaa2 && ch <= (char)IsolatedLetters.Khaa2 + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Dal && ch <= (char)IsolatedLetters.Dal + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Thal && ch <= (char)IsolatedLetters.Thal + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Ra2 && ch <= (char)IsolatedLetters.Ra2 + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Zeen && ch <= (char)IsolatedLetters.Zeen + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Seen && ch <= (char)IsolatedLetters.Seen + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Sheen && ch <= (char)IsolatedLetters.Sheen + 3)
                return true;

            if (ch >= (char)IsolatedLetters.S9a && ch <= (char)IsolatedLetters.S9a + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Dha && ch <= (char)IsolatedLetters.Dha + 3)
                return true;

            if (ch >= (char)IsolatedLetters.T6a && ch <= (char)IsolatedLetters.T6a + 3)
                return true;

            if (ch >= (char)IsolatedLetters.T6ha && ch <= (char)IsolatedLetters.T6ha + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Ain && ch <= (char)IsolatedLetters.Ain + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Gain && ch <= (char)IsolatedLetters.Gain + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Fa && ch <= (char)IsolatedLetters.Fa + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Gaf && ch <= (char)IsolatedLetters.Gaf + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Kaf && ch <= (char)IsolatedLetters.Kaf + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Lam && ch <= (char)IsolatedLetters.Lam + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Meem && ch <= (char)IsolatedLetters.Meem + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Noon && ch <= (char)IsolatedLetters.Noon + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Ha && ch <= (char)IsolatedLetters.Ha + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Waw && ch <= (char)IsolatedLetters.Waw + 3)
                return true;

            if (ch >= (char)IsolatedLetters.Ya && ch <= (char)IsolatedLetters.Ya + 3)
                return true;

            if (ch >= (char)IsolatedLetters.AlefMad && ch <= (char)IsolatedLetters.AlefMad + 3)
                return true;

            if (ch >= (char)IsolatedLetters.TaMarboota && ch <= (char)IsolatedLetters.TaMarboota + 3)
                return true;

            if (ch >= (char)IsolatedLetters.PersianPe && ch <= (char)IsolatedLetters.PersianPe + 3)
                return true;

            if (ch >= (char)IsolatedLetters.PersianChe && ch <= (char)IsolatedLetters.PersianChe + 3)
                return true;

            if (ch >= (char)IsolatedLetters.PersianZe && ch <= (char)IsolatedLetters.PersianZe + 3)
                return true;

            if (ch >= (char)IsolatedLetters.PersianGaf && ch <= (char)IsolatedLetters.PersianGaf + 3)
                return true;

            if (ch >= (char)IsolatedLetters.PersianGaf2 && ch <= (char)IsolatedLetters.PersianGaf2 + 3)
                return true;

            // Special Lam Alef
            if (ch == 0xFEF3)
                return true;

            if (ch == 0xFEF5)
                return true;

            if (ch == 0xFEF7)
                return true;

            if (ch == 0xFEF9)
                return true;

            // Input string that goes to FixGlyph method does not have any general letter.
            // Code below is for IsRTLInput function
            switch (ch)
            {
                case (char)GeneralLetters.Hamza:
                case (char)GeneralLetters.Alef:
                case (char)GeneralLetters.AlefHamza:
                case (char)GeneralLetters.WawHamza:
                case (char)GeneralLetters.AlefMaksoor:
                case (char)GeneralLetters.HamzaNabera:
                case (char)GeneralLetters.Ba:
                case (char)GeneralLetters.Ta:
                case (char)GeneralLetters.Tha2:
                case (char)GeneralLetters.Jeem:
                case (char)GeneralLetters.H7aa:
                case (char)GeneralLetters.Khaa2:
                case (char)GeneralLetters.Dal:
                case (char)GeneralLetters.Thal:
                case (char)GeneralLetters.Ra2:
                case (char)GeneralLetters.Zeen:
                case (char)GeneralLetters.Seen:
                case (char)GeneralLetters.Sheen:
                case (char)GeneralLetters.S9a:
                case (char)GeneralLetters.Dha:
                case (char)GeneralLetters.T6a:
                case (char)GeneralLetters.T6ha:
                case (char)GeneralLetters.Ain:
                case (char)GeneralLetters.Gain:
                case (char)GeneralLetters.Fa:
                case (char)GeneralLetters.Gaf:
                case (char)GeneralLetters.Kaf:
                case (char)GeneralLetters.Lam:
                case (char)GeneralLetters.Meem:
                case (char)GeneralLetters.Noon:
                case (char)GeneralLetters.Ha:
                case (char)GeneralLetters.Waw:
                case (char)GeneralLetters.Ya:
                case (char)GeneralLetters.AlefMad:
                case (char)GeneralLetters.TaMarboota:
                case (char)GeneralLetters.PersianPe:
                case (char)GeneralLetters.PersianChe:
                case (char)GeneralLetters.PersianZe:
                case (char)GeneralLetters.PersianGaf:
                case (char)GeneralLetters.PersianGaf2:
                    return true;
            }

            return false;
        }

        public virtual bool IsRTLInput(string input)
        {
            char[] chars = input.ToCharArray();
            return IsRTLInput(chars);
        }

        public virtual bool IsRTLInput(IEnumerable<char> chars)
        {
            foreach (var character in chars)
            {
                switch (character)
                {
                    // Arabic Tashkeel
                    case (char)0x064B:
                    case (char)0x064C:
                    case (char)0x064D:
                    case (char)0x064E:
                    case (char)0x064F:
                    case (char)0x0650:
                    case (char)0x0651:
                    case (char)0x0652:
                    case (char)0x0653:
                        return true;
                }

                if (char.IsLetter(character))
                {
                    return IsRTLCharacter(character);
                }
            }

            return false;
        }

        protected virtual bool HandleSpecialLam(char[] letters, char[] lettersFinal, int i)
        {
            switch (letters[i + 1])
            {
                case (char)IsolatedLetters.AlefMaksoor:
                    letters[i] = (char)0xFEF7;
                    lettersFinal[i + 1] = (char)0xFFFF;
                    return true;
                case (char)IsolatedLetters.Alef:
                    letters[i] = (char)0xFEF9;
                    lettersFinal[i + 1] = (char)0xFFFF;
                    return true;
                case (char)IsolatedLetters.AlefHamza:
                    letters[i] = (char)0xFEF5;
                    lettersFinal[i + 1] = (char)0xFFFF;
                    return true;
                case (char)IsolatedLetters.AlefMad:
                    letters[i] = (char)0xFEF3;
                    lettersFinal[i + 1] = (char)0xFFFF;
                    return true;
            }

            return false;
        }

        protected virtual bool IsLeadingLetter(IList<char> letters, int index)
        {
            bool previousLetterCheck = index == 0 ||
                                       IsRTLCharacter(letters[index - 1]) == false ||
                                       letters[index - 1] == (int)IsolatedLetters.Alef ||
                                       letters[index - 1] == (int)IsolatedLetters.Dal ||
                                       letters[index - 1] == (int)IsolatedLetters.Thal ||
                                       letters[index - 1] == (int)IsolatedLetters.Ra2 ||
                                       letters[index - 1] == (int)IsolatedLetters.Zeen ||
                                       letters[index - 1] == (int)IsolatedLetters.PersianZe ||
                                       letters[index - 1] == (int)IsolatedLetters.Waw ||
                                       letters[index - 1] == (int)IsolatedLetters.AlefMad ||
                                       letters[index - 1] == (int)IsolatedLetters.AlefHamza ||
                                       letters[index - 1] == (int)IsolatedLetters.Hamza ||
                                       letters[index - 1] == (int)IsolatedLetters.AlefMaksoor ||
                                       letters[index - 1] == (int)IsolatedLetters.WawHamza;

            bool leadingLetterCheck = letters[index] != ' ' &&
                                      letters[index] != (int)IsolatedLetters.Dal &&
                                      letters[index] != (int)IsolatedLetters.Thal &&
                                      letters[index] != (int)IsolatedLetters.Ra2 &&
                                      letters[index] != (int)IsolatedLetters.Zeen &&
                                      letters[index] != (int)IsolatedLetters.PersianZe &&
                                      letters[index] != (int)IsolatedLetters.Alef &&
                                      letters[index] != (int)IsolatedLetters.AlefHamza &&
                                      letters[index] != (int)IsolatedLetters.AlefMaksoor &&
                                      letters[index] != (int)IsolatedLetters.AlefMad &&
                                      letters[index] != (int)IsolatedLetters.WawHamza &&
                                      letters[index] != (int)IsolatedLetters.Waw &&
                                      letters[index] != (int)IsolatedLetters.Hamza;

            bool nextLetterCheck = index < letters.Count - 1 &&
                                   IsRTLCharacter(letters[index + 1]) &&
                                   letters[index + 1] != (int)IsolatedLetters.Hamza;

            return previousLetterCheck && leadingLetterCheck && nextLetterCheck;
        }

        protected virtual bool IsFinishingLetter(IList<char> letters, int index)
        {
            bool previousLetterCheck = index != 0 &&
                                       letters[index - 1] != ' ' &&
                                       letters[index - 1] != (int)IsolatedLetters.Dal &&
                                       letters[index - 1] != (int)IsolatedLetters.Thal &&
                                       letters[index - 1] != (int)IsolatedLetters.Ra2 &&
                                       letters[index - 1] != (int)IsolatedLetters.Zeen &&
                                       letters[index - 1] != (int)IsolatedLetters.PersianZe &&
                                       letters[index - 1] != (int)IsolatedLetters.Waw &&
                                       letters[index - 1] != (int)IsolatedLetters.Alef &&
                                       letters[index - 1] != (int)IsolatedLetters.AlefMad &&
                                       letters[index - 1] != (int)IsolatedLetters.AlefHamza &&
                                       letters[index - 1] != (int)IsolatedLetters.AlefMaksoor &&
                                       letters[index - 1] != (int)IsolatedLetters.WawHamza &&
                                       letters[index - 1] != (int)IsolatedLetters.Hamza &&
                                       IsRTLCharacter(letters[index - 1]);


            bool finishingLetterCheck = letters[index] != ' ' && letters[index] != (int)IsolatedLetters.Hamza;


            return previousLetterCheck && finishingLetterCheck;
        }

        protected virtual bool IsMiddleLetter(IList<char> letters, int index)
        {
            bool middleLetterCheck = index != 0 &&
                                     letters[index] != (int)IsolatedLetters.Alef &&
                                     letters[index] != (int)IsolatedLetters.Dal &&
                                     letters[index] != (int)IsolatedLetters.Thal &&
                                     letters[index] != (int)IsolatedLetters.Ra2 &&
                                     letters[index] != (int)IsolatedLetters.Zeen &&
                                     letters[index] != (int)IsolatedLetters.PersianZe &&
                                     letters[index] != (int)IsolatedLetters.Waw &&
                                     letters[index] != (int)IsolatedLetters.AlefMad &&
                                     letters[index] != (int)IsolatedLetters.AlefHamza &&
                                     letters[index] != (int)IsolatedLetters.AlefMaksoor &&
                                     letters[index] != (int)IsolatedLetters.WawHamza &&
                                     letters[index] != (int)IsolatedLetters.Hamza;

            bool previousLetterCheck = index != 0 &&
                                       letters[index - 1] != (int)IsolatedLetters.Alef &&
                                       letters[index - 1] != (int)IsolatedLetters.Dal &&
                                       letters[index - 1] != (int)IsolatedLetters.Thal &&
                                       letters[index - 1] != (int)IsolatedLetters.Ra2 &&
                                       letters[index - 1] != (int)IsolatedLetters.Zeen &&
                                       letters[index - 1] != (int)IsolatedLetters.PersianZe &&
                                       letters[index - 1] != (int)IsolatedLetters.Waw &&
                                       letters[index - 1] != (int)IsolatedLetters.AlefMad &&
                                       letters[index - 1] != (int)IsolatedLetters.AlefHamza &&
                                       letters[index - 1] != (int)IsolatedLetters.AlefMaksoor &&
                                       letters[index - 1] != (int)IsolatedLetters.WawHamza &&
                                       letters[index - 1] != (int)IsolatedLetters.Hamza &&
                                       IsRTLCharacter(letters[index - 1]);

            bool nextLetterCheck = index < letters.Count - 1 &&
                                   IsRTLCharacter(letters[index + 1]) &&
                                   letters[index + 1] != (int)IsolatedLetters.Hamza;

            return nextLetterCheck && previousLetterCheck && middleLetterCheck;
        }
    }
}