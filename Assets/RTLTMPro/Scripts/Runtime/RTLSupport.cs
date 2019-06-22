/*
 * This plugin is made possible by Arabic Support plugin created by: Abdulla Konash. Twitter: @konash
 * Original Arabic Support can be found here: https://github.com/Konash/arabic-support-unity
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;

namespace RTLTMPro
{
    public static class RTLSupport
    {
        private static readonly List<TashkeelLocation> TashkeelLocations;
        private static readonly Regex PairedTagFixer;
        private static readonly Regex LoneTagFixer;
        private static readonly StringBuilder FinalLetters;

        private static readonly string ShaddaDammatan = new string(
            new[] {(char) TashkeelCharacters.Shadda, (char) TashkeelCharacters.Dammatan});

        private static readonly string ShaddaKasratan = new string(
            new[] {(char) TashkeelCharacters.Shadda, (char) TashkeelCharacters.Kasratan});

        private static readonly string ShaddaSuperscriptAlef = new string(
            new[] {(char) TashkeelCharacters.Shadda, (char) TashkeelCharacters.SuperscriptAlef});

        private static readonly string ShaddaFatha = new string(
            new[] {(char) TashkeelCharacters.Shadda, (char) TashkeelCharacters.Fatha});

        private static readonly string ShaddaDamma = new string(
            new[] {(char) TashkeelCharacters.Shadda, (char) TashkeelCharacters.Damma});

        private static readonly string ShaddaKasra = new string(
            new[] {(char) TashkeelCharacters.Shadda, (char) TashkeelCharacters.Kasra});

        static RTLSupport()
        {
            FinalLetters = new StringBuilder();
            TashkeelLocations = new List<TashkeelLocation>();
            PairedTagFixer = new Regex(
                @"(?<closing></(?<tagName>\p{Ll}+)>)(?<content>(.|\n)+?)(?<opening><\k<tagName>=?(\p{L}|\p{N}|-|\+|#)*>)");
            LoneTagFixer = new Regex(@"(?<!</\p{Ll}+>.*)(<\p{Ll}+=?(\p{Ll}|\p{N})+/?>)");
        }

        /// <summary>
        ///     Fixes the provided string
        /// </summary>
        /// <param name="input">Text to fix</param>
        /// <param name="fixTextTags"></param>
        /// <param name="preserveNumbers"></param>
        /// <param name="farsi"></param>
        /// <returns>Fixed text</returns>
        public static string FixRTL(string input, bool fixTextTags, bool preserveNumbers, bool farsi)
        {
            FinalLetters.Length = 0;
            TashkeelLocations.Clear();

            // Prepared text does not have tashkeel. Also all letters are converted into isolated from. 
            // NOTE: Prepared Letters array is reversed.
            char[] preparedLetters = PrepareInput(input, farsi);

            // The shape of the letters in shapeFixedLetters is fixed according to their position in word. But the flow of the text is not fixed.
            char[] shapeFixedLetters = FixGlyphs(preparedLetters, preserveNumbers, farsi);

            // Fix flow of the text and put the result in FinalLetters field
            FixLigature(shapeFixedLetters, fixTextTags);
            input = FinalLetters.ToString();
            if (fixTextTags)
                input = FixTags(input);

            return input;
        }

        /// <summary>
        ///     Checks if the input string starts with supported RTL character or not.
        /// </summary>
        /// <returns><see langword="true" /> if input is RTL. otherwise <see langword="false" /></returns>
        public static bool IsRTLInput(string input)
        {
            return IsRTLInput((IEnumerable<char>) input);
        }

        /// <summary>
        ///     Checks if the input character array starts with supported RTL character or not.
        /// </summary>
        /// <returns><see langword="true" /> if input is RTL. otherwise <see langword="false" /></returns>
        public static bool IsRTLInput(IEnumerable<char> chars)
        {
            bool insideTag = false;
            foreach (var character in chars)
            {
                switch (character)
                {
                    case '<':
                        insideTag = true;
                        continue;
                    case '>':
                        insideTag = false;
                        continue;

                    // Arabic Tashkeel
                    case (char) TashkeelCharacters.Fathan:
                    case (char) TashkeelCharacters.Dammatan:
                    case (char) TashkeelCharacters.Kasratan:
                    case (char) TashkeelCharacters.Fatha:
                    case (char) TashkeelCharacters.Damma:
                    case (char) TashkeelCharacters.Kasra:
                    case (char) TashkeelCharacters.Shadda:
                    case (char) TashkeelCharacters.Sukun:
                    case (char) TashkeelCharacters.MaddahAbove:
                        return true;
                }

                if (insideTag)
                    continue;

                if (char.IsLetter(character))
                {
                    return IsRTLCharacter(character);
                }
            }

            return false;
        }

        /// <summary>
        ///     Fixes rich text tags in input string and returns the result.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Fixed text</returns>
        private static string FixTags(string input)
        {
            var tags = PairedTagFixer.Matches(input);
            foreach (Match match in tags)
            {
                // Opening and closing tags need to be reversed. 
                // Also, we need to swap opening and closing tags.

                input = input.Remove(match.Index, match.Length);
                string opening = match.Groups["opening"].Value.Reverse().ToArray().ArrayToString();
                string closing = match.Groups["closing"].Value.Reverse().ToArray().ArrayToString();
                string content = match.Groups["content"].Value;

                input = input.Insert(match.Index, closing);
                input = input.Insert(match.Index + closing.Length, content);
                input = input.Insert(match.Index + closing.Length + content.Length, opening);
            }

            tags = LoneTagFixer.Matches(input);
            foreach (Match match in tags)
            {
                // Lone tags need to be reversed.
                input = input.Remove(match.Index, match.Length);
                string opening = match.Value.Reverse().ToArray().ArrayToString();
                input = input.Insert(match.Index, opening);
            }

            return input;
        }

        /// <summary>
        ///     Removes tashkeel. Converts general RTL letters to isolated form. Also fixes Farsi and Arabic ی letter.
        /// </summary>
        /// <param name="input">Input to prepare</param>
        /// <param name="farsi"></param>
        /// <returns>Prepared input in char array</returns>
        private static char[] PrepareInput(string input, bool farsi)
        {
            input = RemoveTashkeel(input);
            char[] letters = input.ToCharArray();
            for (int i = 0; i < letters.Length; i++)
            {
                if (farsi && letters[i] == (int) GeneralLetters.Ya)
                {
                    letters[i] = (char) GeneralLetters.PersianYa;
                }
                else if (farsi == false && letters[i] == (int) GeneralLetters.PersianYa)
                {
                    letters[i] = (char) GeneralLetters.Ya;
                }
            }

            return letters;
        }

        /// <summary>
        ///     Removes tashkeel from text.
        /// </summary>
        private static string RemoveTashkeel(string str)
        {
            char[] letters = str.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                switch ((TashkeelCharacters) letters[i])
                {
                    case TashkeelCharacters.Fathan:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Fathan, i));
                        break;
                    case TashkeelCharacters.Dammatan:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Dammatan, i));
                        break;
                    case TashkeelCharacters.Kasratan:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Kasratan, i));
                        break;
                    case TashkeelCharacters.Fatha:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Fatha, i));
                        break;
                    case TashkeelCharacters.Damma:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Damma, i));
                        break;
                    case TashkeelCharacters.Kasra:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Kasra, i));
                        break;
                    case TashkeelCharacters.Shadda:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Shadda, i));
                        break;
                    case TashkeelCharacters.Sukun:
                        TashkeelLocations.Add(new TashkeelLocation(TashkeelCharacters.Sukun, i));
                        break;
                    case TashkeelCharacters.MaddahAbove:
                        TashkeelLocations.Add(
                            new TashkeelLocation(TashkeelCharacters.MaddahAbove, i));
                        break;
                    case TashkeelCharacters.SuperscriptAlef:
                        TashkeelLocations.Add(
                            new TashkeelLocation(TashkeelCharacters.SuperscriptAlef, i));
                        break;
                }
            }

            string[] split = str.Split(
                (char) TashkeelCharacters.Fathan,
                (char) TashkeelCharacters.Dammatan,
                (char) TashkeelCharacters.Kasratan,
                (char) TashkeelCharacters.Fatha,
                (char) TashkeelCharacters.Damma,
                (char) TashkeelCharacters.Kasra,
                (char) TashkeelCharacters.Shadda,
                (char) TashkeelCharacters.Sukun,
                (char) TashkeelCharacters.MaddahAbove,
                (char) TashkeelCharacters.ShaddaWithFathaIsolatedForm,
                (char) TashkeelCharacters.ShaddaWithDammaIsolatedForm,
                (char) TashkeelCharacters.ShaddaWithKasraIsolatedForm,
                (char) TashkeelCharacters.SuperscriptAlef);

            return split.Aggregate("", (current, s) => current + s);
        }

        /// <summary>
        ///     Fixes the shape of letters based on their position.
        /// </summary>
        /// <param name="letters"></param>
        /// <param name="preserveNumbers"></param>
        /// <param name="farsi"></param>
        /// <returns></returns>
        private static char[] FixGlyphs(char[] letters, bool preserveNumbers, bool farsi)
        {
            // We copy the letters into final letters and fix the ones that actually need a fix
            char[] lettersFinal = new char[letters.Length];
            Array.Copy(letters, lettersFinal, letters.Length);

            for (int i = 0; i < letters.Length; i++)
            {
                bool skipNext = false;

                // For special Lam Letter connections.
                if (letters[i] == (char) GeneralLetters.Lam)
                {
                    if (i < letters.Length - 1)
                    {
                        skipNext = HandleSpecialLam(letters, lettersFinal, i);
                    }
                }

                // We don't want to fix tatweel or zwnj character
                if (letters[i] == (int) GeneralLetters.ArabicTatweel ||
                    letters[i] == (int) GeneralLetters.ZeroWidthNoJoiner)
                    continue;

                if (IsRTLCharacter(letters[i]))
                {
                    if (IsMiddleLetter(letters, i))
                    {
                        letters[i] = GlyphTable.Convert(letters[i]);
                        lettersFinal[i] = (char) (letters[i] + 3);
                    }
                    else if (IsFinishingLetter(letters, i))
                    {
                        letters[i] = GlyphTable.Convert(letters[i]);
                        lettersFinal[i] = (char) (letters[i] + 1);
                    }
                    else if (IsLeadingLetter(letters, i))
                    {
                        letters[i] = GlyphTable.Convert(letters[i]);
                        lettersFinal[i] = (char) (letters[i] + 2);
                    }
                }


                // If this letter as Lam and special Lam-Alef connection was made, We want to skip the Alef
                // (Lam-Alef occupies 1 space)
                if (skipNext)
                {
                    i++;
                    continue;
                }

                if (!preserveNumbers && char.IsDigit(letters[i]))
                {
                    lettersFinal[i] = FixNumbers(letters[i], farsi);
                }
            }

            //Restore tashkeel to their places.
            lettersFinal = RestoreTashkeel(lettersFinal);
            return lettersFinal;
        }

        /// <summary>
        ///     Converts English numbers to Persian or Arabic numbers. 
        /// </summary>
        /// <param name="num">Number to convert.</param>
        /// <param name="farsi"></param>
        /// <returns>Converted number</returns>
        private static char FixNumbers(char num, bool farsi)
        {
            switch ((EnglishNumbers) num)
            {
                case EnglishNumbers.Zero:
                    return farsi ? (char) FarsiNumbers.Zero : (char) HinduNumbers.Zero;
                case EnglishNumbers.One:
                    return farsi ? (char) FarsiNumbers.One : (char) HinduNumbers.One;
                case EnglishNumbers.Two:
                    return farsi ? (char) FarsiNumbers.Two : (char) HinduNumbers.Two;
                case EnglishNumbers.Three:
                    return farsi ? (char) FarsiNumbers.Three : (char) HinduNumbers.Three;
                case EnglishNumbers.Four:
                    return farsi ? (char) FarsiNumbers.Four : (char) HinduNumbers.Four;
                case EnglishNumbers.Five:
                    return farsi ? (char) FarsiNumbers.Five : (char) HinduNumbers.Five;
                case EnglishNumbers.Six:
                    return farsi ? (char) FarsiNumbers.Six : (char) HinduNumbers.Six;
                case EnglishNumbers.Seven:
                    return farsi ? (char) FarsiNumbers.Seven : (char) HinduNumbers.Seven;
                case EnglishNumbers.Eight:
                    return farsi ? (char) FarsiNumbers.Eight : (char) HinduNumbers.Eight;
                case EnglishNumbers.Nine:
                    return farsi ? (char) FarsiNumbers.Nine : (char) HinduNumbers.Nine;
            }

            return num;
        }

        /// <summary>
        ///     Fixes the flow of the text and stores the output in <see cref="FinalLetters" /> list.
        /// </summary>
        /// <param name="shapeFixedLetters">Output of <see cref="FixGlyphs" /> method.</param>
        /// <param name="fixTextTags"></param>
        private static void FixLigature(IList<char> shapeFixedLetters, bool fixTextTags)
        {
            // NOTE: shapeFixedLetters are in reversed order. 0th element is the last character of the text.

            // Some texts like tags, English words and numbers need to be displayed in their original order.
            // This list keeps the characters that their order should be reserved and streams reserved texts into final letters.
            List<char> ltrText = new List<char>();
            for (int i = shapeFixedLetters.Count - 1; i >= 0; i--)
            {
                bool isInMiddle = i > 0 && i < shapeFixedLetters.Count - 1;
                bool isAtBegining = i == shapeFixedLetters.Count - 1;
                bool isAtEnd = i == 0;

                if (char.IsPunctuation(shapeFixedLetters[i]) || char.IsSymbol(shapeFixedLetters[i]))
                {
                    if (fixTextTags)
                    {
                        if (shapeFixedLetters[i] == '>')
                        {
                            // We need to check if it is actually the begining of a tag.
                            bool valid = false;
                            // If > is at the end of the text (At begining of the array), it can't be a tag
                            if (isAtEnd == false)
                            {
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    // Tags do not have space inside
                                    if (shapeFixedLetters[j] == ' ')
                                    {
                                        break;
                                    }

                                    // Tags do not have RTL characters inside
                                    if (IsRTLCharacter(shapeFixedLetters[j]))
                                        break;

                                    if (shapeFixedLetters[j] == '<')
                                    {
                                        valid = true;
                                        break;
                                    }
                                }
                            }

                            if (ltrText.Count > 0 && valid)
                            {
                                for (int j = 0; j < ltrText.Count; j++)
                                    FinalLetters.Append(ltrText[ltrText.Count - 1 - j]);
                                ltrText.Clear();
                            }
                        }
                    }

                    if (shapeFixedLetters[i] == ')')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                                shapeFixedLetters[i] = '(';
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            if (isAfterRTLCharacter)
                                shapeFixedLetters[i] = '(';
                        }
                        else if (isAtBegining)
                        {
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);
                            if (isBeforeRTLCharacter)
                                shapeFixedLetters[i] = '(';
                        }
                    }
                    else if (shapeFixedLetters[i] == '(')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                                shapeFixedLetters[i] = ')';
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            if (isAfterRTLCharacter)
                                shapeFixedLetters[i] = ')';
                        }
                        else if (isAtBegining)
                        {
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);
                            if (isBeforeRTLCharacter)
                                shapeFixedLetters[i] = ')';
                        }
                    }
                    else if (shapeFixedLetters[i] == '«')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                                shapeFixedLetters[i] = '»';
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            if (isAfterRTLCharacter)
                                shapeFixedLetters[i] = '»';
                        }
                        else if (isAtBegining)
                        {
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);
                            if (isBeforeRTLCharacter)
                                shapeFixedLetters[i] = '»';
                        }
                    }
                    else if (shapeFixedLetters[i] == '»')
                    {
                        if (isInMiddle)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);

                            if (isAfterRTLCharacter || isBeforeRTLCharacter)
                                shapeFixedLetters[i] = '«';
                        }
                        else if (isAtEnd)
                        {
                            bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                            if (isAfterRTLCharacter)
                                shapeFixedLetters[i] = '«';
                        }
                        else if (isAtBegining)
                        {
                            bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);
                            if (isBeforeRTLCharacter)
                                shapeFixedLetters[i] = '«';
                        }
                    }

                    if (isInMiddle)
                    {
                        // NOTE: Array is reversed. i + 1 is behind and i - 1 is ahead
                        bool isAfterRTLCharacter = IsRTLCharacter(shapeFixedLetters[i + 1]);
                        bool isBeforeRTLCharacter = IsRTLCharacter(shapeFixedLetters[i - 1]);
                        bool isBeforeWhiteSpace = char.IsWhiteSpace(shapeFixedLetters[i - 1]);
                        bool isAfterWhiteSpace = char.IsWhiteSpace(shapeFixedLetters[i + 1]);
                        bool isUnderline = shapeFixedLetters[i] == '_';
                        bool isSpecialPunctuation = shapeFixedLetters[i] == '.' ||
                                                    shapeFixedLetters[i] == '،' ||
                                                    shapeFixedLetters[i] == '؛';

                        if (isBeforeRTLCharacter && isAfterRTLCharacter ||
                            isAfterWhiteSpace && isSpecialPunctuation ||
                            isBeforeWhiteSpace && isAfterRTLCharacter ||
                            isBeforeRTLCharacter && isAfterWhiteSpace ||
                            ((isBeforeRTLCharacter || isAfterRTLCharacter ) && isUnderline))
                        {
                            if (ltrText.Count > 0)
                            {
                                for (int j = 0; j < ltrText.Count; j++)
                                    FinalLetters.Append(ltrText[ltrText.Count - 1 - j]);
                                ltrText.Clear();
                            }

                            FinalLetters.Append(shapeFixedLetters[i]);
                        }
                        else
                        {
                            ltrText.Add(shapeFixedLetters[i]);
                        }
                    }
                    else if (isAtEnd)
                    {
                        FinalLetters.Append(shapeFixedLetters[i]);
                    }
                    else if (isAtBegining)
                    {
                        ltrText.Add(shapeFixedLetters[i]);
                    }

                    if (fixTextTags)
                    {
                        if (shapeFixedLetters[i] == '<')
                        {
                            bool valid = false;

                            if (isAtBegining == false)
                            {
                                for (int j = i + 1; j < shapeFixedLetters.Count; j++)
                                {
                                    // Tags do not have space inside
                                    if (shapeFixedLetters[j] == ' ')
                                    {
                                        break;
                                    }

                                    // Tags do not have RTL characters inside
                                    if (IsRTLCharacter(shapeFixedLetters[j]))
                                        break;

                                    if (shapeFixedLetters[j] == '>')
                                    {
                                        valid = true;
                                        break;
                                    }
                                }
                            }

                            if (ltrText.Count > 0 && valid)
                            {
                                for (int j = 0; j < ltrText.Count; j++)
                                    FinalLetters.Append(ltrText[ltrText.Count - 1 - j]);
                                ltrText.Clear();
                            }
                        }
                    }

                    continue;
                }

                if (isInMiddle)
                {
                    bool isAfterEnglishChar = char.IsLower(shapeFixedLetters[i + 1]) ||
                                              char.IsUpper(shapeFixedLetters[i + 1]);
                    bool isBeforeEnglishChar =
                        char.IsLower(shapeFixedLetters[i - 1]) ||
                        char.IsUpper(shapeFixedLetters[i - 1]);
                    bool isAfterNumber = char.IsNumber(shapeFixedLetters[i + 1]);
                    bool isBeforeNumber = char.IsNumber(shapeFixedLetters[i - 1]);
                    bool isAfterSymbol = char.IsSymbol(shapeFixedLetters[i + 1]);
                    bool isBeforeSymbol = char.IsSymbol(shapeFixedLetters[i - 1]);

                    // For cases where english words and arabic are mixed. This allows for using arabic, english and numbers in one sentence.
                    // If the space is between numbers,symbols or English words, keep the order
                    if (shapeFixedLetters[i] == ' ' &&
                        (isBeforeEnglishChar || isBeforeNumber || isBeforeSymbol) &&
                        (isAfterEnglishChar || isAfterNumber || isAfterSymbol))
                    {
                        ltrText.Add(shapeFixedLetters[i]);
                        continue;
                    }
                }

                if (char.IsNumber(shapeFixedLetters[i]) ||
                    char.IsLower(shapeFixedLetters[i]) ||
                    char.IsUpper(shapeFixedLetters[i]))
                {
                    ltrText.Add(shapeFixedLetters[i]);
                    continue;
                }

                if (shapeFixedLetters[i] >= (char) 0xD800 &&
                    shapeFixedLetters[i] <= (char) 0xDBFF ||
                    shapeFixedLetters[i] >= (char) 0xDC00 && shapeFixedLetters[i] <= (char) 0xDFFF)
                {
                    ltrText.Add(shapeFixedLetters[i]);
                    continue;
                }

                if (ltrText.Count > 0)
                {
                    for (int j = 0; j < ltrText.Count; j++)
                        FinalLetters.Append(ltrText[ltrText.Count - 1 - j]);
                    ltrText.Clear();
                }

                if (shapeFixedLetters[i] != 0xFFFF &&
                    shapeFixedLetters[i] != (int) GeneralLetters.ZeroWidthNoJoiner)
                    FinalLetters.Append(shapeFixedLetters[i]);
            }

            if (ltrText.Count > 0)
            {
                for (int j = 0; j < ltrText.Count; j++)
                    FinalLetters.Append(ltrText[ltrText.Count - 1 - j]);
                ltrText.Clear();
            }
        }

        /// <summary>
        ///     Restores removed tashkeel.
        /// </summary>
        private static char[] RestoreTashkeel(ICollection<char> letters)
        {
            var lettersWithTashkeel = new StringBuilder();

            int letterWithTashkeelTracker = 0;
            foreach (char t in letters)
            {
                lettersWithTashkeel.Append(t);
                letterWithTashkeelTracker++;

                foreach (TashkeelLocation hLocation in TashkeelLocations)
                {
                    if (hLocation.Position != letterWithTashkeelTracker)
                        continue;

                    lettersWithTashkeel.Append(hLocation.Tashkeel);
                    letterWithTashkeelTracker++;
                }
            }

            /*
             * Fix of https://github.com/sorencoder/RTLTMPro/issues/13
             * The workaround is to replace Shadda + Aother Tashkeel with combined form 
             */
            lettersWithTashkeel.Replace(
                ShaddaFatha,
                ((char) TashkeelCharacters.ShaddaWithFathaIsolatedForm).ToString());

            lettersWithTashkeel.Replace(
                ShaddaDamma,
                ((char) TashkeelCharacters.ShaddaWithDammaIsolatedForm).ToString());

            lettersWithTashkeel.Replace(
                ShaddaKasra,
                ((char) TashkeelCharacters.ShaddaWithKasraIsolatedForm).ToString());

            lettersWithTashkeel.Replace(
                ShaddaDammatan,
                ((char) TashkeelCharacters.ShaddaWithDammatanIsolatedForm).ToString());

            lettersWithTashkeel.Replace(
                ShaddaKasratan,
                ((char) TashkeelCharacters.ShaddaWithKasratanIsolatedForm).ToString());

            lettersWithTashkeel.Replace(
                ShaddaSuperscriptAlef,
                ((char) TashkeelCharacters.ShaddaWithSuperscriptAlefIsolatedForm).ToString());

            // TODO: Probably there is better way of converting string to char[]
            var result = new char[lettersWithTashkeel.Length];
            lettersWithTashkeel.CopyTo(0, result, 0, lettersWithTashkeel.Length);
            return result;
        }

        /// <summary>
        ///     Handles the special Lam-Alef connection in the text.
        /// </summary>
        /// <param name="letters">Original array</param>
        /// <param name="lettersFinal">Result array</param>
        /// <param name="i">Index of Lam letter</param>
        /// <returns><see langword="true" /> if special connection has been made.</returns>
        private static bool HandleSpecialLam(char[] letters, char[] lettersFinal, int i)
        {
            switch (letters[i + 1])
            {
                case (char) GeneralLetters.AlefMaksoor:
                    letters[i] = (char) 0xFEF7;
                    lettersFinal[i + 1] = (char) 0xFFFF;
                    return true;
                case (char) GeneralLetters.Alef:
                    letters[i] = (char) 0xFEF9;
                    lettersFinal[i + 1] = (char) 0xFFFF;
                    return true;
                case (char) GeneralLetters.AlefHamza:
                    letters[i] = (char) 0xFEF5;
                    lettersFinal[i + 1] = (char) 0xFFFF;
                    return true;
                case (char) GeneralLetters.AlefMad:
                    letters[i] = (char) 0xFEF3;
                    lettersFinal[i + 1] = (char) 0xFFFF;
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks if the character is supported RTL character.
        /// </summary>
        /// <param name="ch">Character to check</param>
        /// <returns><see langword="true" /> if character is supported. otherwise <see langword="false" /></returns>
        public static bool IsRTLCharacter(char ch)
        {
            /*
             * Other shapes of each letter comes right after the isolated form.
             * That's why we add 3 to the isolated letter to cover every shape the letter
             */

            if (ch >= (char) IsolatedLetters.Hamza && ch <= (char) IsolatedLetters.Hamza + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Alef && ch <= (char) IsolatedLetters.Alef + 3)
                return true;

            if (ch >= (char) IsolatedLetters.AlefHamza &&
                ch <= (char) IsolatedLetters.AlefHamza + 3)
                return true;

            if (ch >= (char) IsolatedLetters.WawHamza && ch <= (char) IsolatedLetters.WawHamza + 3)
                return true;

            if (ch >= (char) IsolatedLetters.AlefMaksoor &&
                ch <= (char) IsolatedLetters.AlefMaksoor + 3)
                return true;

            if (ch >= (char) IsolatedLetters.AlefMaksura &&
                ch <= (char) IsolatedLetters.AlefMaksura + 3)
                return true;

            if (ch >= (char) IsolatedLetters.HamzaNabera &&
                ch <= (char) IsolatedLetters.HamzaNabera + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Ba && ch <= (char) IsolatedLetters.Ba + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Ta && ch <= (char) IsolatedLetters.Ta + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Tha2 && ch <= (char) IsolatedLetters.Tha2 + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Jeem && ch <= (char) IsolatedLetters.Jeem + 3)
                return true;

            if (ch >= (char) IsolatedLetters.H7aa && ch <= (char) IsolatedLetters.H7aa + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Khaa2 && ch <= (char) IsolatedLetters.Khaa2 + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Dal && ch <= (char) IsolatedLetters.Dal + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Thal && ch <= (char) IsolatedLetters.Thal + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Ra2 && ch <= (char) IsolatedLetters.Ra2 + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Zeen && ch <= (char) IsolatedLetters.Zeen + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Seen && ch <= (char) IsolatedLetters.Seen + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Sheen && ch <= (char) IsolatedLetters.Sheen + 3)
                return true;

            if (ch >= (char) IsolatedLetters.S9a && ch <= (char) IsolatedLetters.S9a + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Dha && ch <= (char) IsolatedLetters.Dha + 3)
                return true;

            if (ch >= (char) IsolatedLetters.T6a && ch <= (char) IsolatedLetters.T6a + 3)
                return true;

            if (ch >= (char) IsolatedLetters.T6ha && ch <= (char) IsolatedLetters.T6ha + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Ain && ch <= (char) IsolatedLetters.Ain + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Gain && ch <= (char) IsolatedLetters.Gain + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Fa && ch <= (char) IsolatedLetters.Fa + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Gaf && ch <= (char) IsolatedLetters.Gaf + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Kaf && ch <= (char) IsolatedLetters.Kaf + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Lam && ch <= (char) IsolatedLetters.Lam + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Meem && ch <= (char) IsolatedLetters.Meem + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Noon && ch <= (char) IsolatedLetters.Noon + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Ha && ch <= (char) IsolatedLetters.Ha + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Waw && ch <= (char) IsolatedLetters.Waw + 3)
                return true;

            if (ch >= (char) IsolatedLetters.Ya && ch <= (char) IsolatedLetters.Ya + 3)
                return true;

            if (ch >= (char) IsolatedLetters.AlefMad && ch <= (char) IsolatedLetters.AlefMad + 3)
                return true;

            if (ch >= (char) IsolatedLetters.TaMarboota &&
                ch <= (char) IsolatedLetters.TaMarboota + 3)
                return true;

            if (ch >= (char) IsolatedLetters.PersianPe &&
                ch <= (char) IsolatedLetters.PersianPe + 3)
                return true;

            if (ch >= (char) IsolatedLetters.PersianYa &&
                ch <= (char) IsolatedLetters.PersianYa + 3)
                return true;

            if (ch >= (char) IsolatedLetters.PersianChe &&
                ch <= (char) IsolatedLetters.PersianChe + 3)
                return true;

            if (ch >= (char) IsolatedLetters.PersianZe &&
                ch <= (char) IsolatedLetters.PersianZe + 3)
                return true;

            if (ch >= (char) IsolatedLetters.PersianGaf &&
                ch <= (char) IsolatedLetters.PersianGaf + 3)
                return true;

            if (ch >= (char) IsolatedLetters.PersianGaf2 &&
                ch <= (char) IsolatedLetters.PersianGaf2 + 3)
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

            switch (ch)
            {
                case (char) GeneralLetters.Hamza:
                case (char) GeneralLetters.Alef:
                case (char) GeneralLetters.AlefHamza:
                case (char) GeneralLetters.WawHamza:
                case (char) GeneralLetters.AlefMaksoor:
                case (char) GeneralLetters.AlefMaksura:
                case (char) GeneralLetters.HamzaNabera:
                case (char) GeneralLetters.Ba:
                case (char) GeneralLetters.Ta:
                case (char) GeneralLetters.Tha2:
                case (char) GeneralLetters.Jeem:
                case (char) GeneralLetters.H7aa:
                case (char) GeneralLetters.Khaa2:
                case (char) GeneralLetters.Dal:
                case (char) GeneralLetters.Thal:
                case (char) GeneralLetters.Ra2:
                case (char) GeneralLetters.Zeen:
                case (char) GeneralLetters.Seen:
                case (char) GeneralLetters.Sheen:
                case (char) GeneralLetters.S9a:
                case (char) GeneralLetters.Dha:
                case (char) GeneralLetters.T6a:
                case (char) GeneralLetters.T6ha:
                case (char) GeneralLetters.Ain:
                case (char) GeneralLetters.Gain:
                case (char) GeneralLetters.Fa:
                case (char) GeneralLetters.Gaf:
                case (char) GeneralLetters.Kaf:
                case (char) GeneralLetters.Lam:
                case (char) GeneralLetters.Meem:
                case (char) GeneralLetters.Noon:
                case (char) GeneralLetters.Ha:
                case (char) GeneralLetters.Waw:
                case (char) GeneralLetters.Ya:
                case (char) GeneralLetters.AlefMad:
                case (char) GeneralLetters.TaMarboota:
                case (char) GeneralLetters.PersianPe:
                case (char) GeneralLetters.PersianChe:
                case (char) GeneralLetters.PersianZe:
                case (char) GeneralLetters.PersianGaf:
                case (char) GeneralLetters.PersianGaf2:
                case (char) GeneralLetters.PersianYa:
                case (char) GeneralLetters.ArabicTatweel:
                case (char) GeneralLetters.ZeroWidthNoJoiner:
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Is the letter at provided index a leading letter?
        /// </summary>
        /// <returns><see langword="true" /> if the letter is a leading letter</returns>
        private static bool IsLeadingLetter(IList<char> letters, int index)
        {
            bool isPreviousLetterNonConnectable = index == 0 ||
                                                  !IsRTLCharacter(letters[index - 1]) ||
                                                  letters[index - 1] == (int) GeneralLetters.Alef ||
                                                  letters[index - 1] == (int) GeneralLetters.Dal ||
                                                  letters[index - 1] == (int) GeneralLetters.Thal ||
                                                  letters[index - 1] == (int) GeneralLetters.Ra2 ||
                                                  letters[index - 1] == (int) GeneralLetters.Zeen ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.PersianZe ||
                                                  letters[index - 1] == (int) GeneralLetters.Waw ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.AlefMad ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.AlefHamza ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.Hamza ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.AlefMaksoor ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.ZeroWidthNoJoiner ||
                                                  letters[index - 1] ==
                                                  (int) GeneralLetters.WawHamza ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.Alef ||
                                                  letters[index - 1] == (int) IsolatedLetters.Dal ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.Thal ||
                                                  letters[index - 1] == (int) IsolatedLetters.Ra2 ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.Zeen ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.PersianZe ||
                                                  letters[index - 1] == (int) IsolatedLetters.Waw ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.AlefMad ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.AlefHamza ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.Hamza ||
                                                  letters[index - 1] ==
                                                  (int) IsolatedLetters.AlefMaksoor;

            bool canThisLetterBeLeading = letters[index] != ' ' &&
                                          letters[index] != (int) GeneralLetters.Dal &&
                                          letters[index] != (int) GeneralLetters.Thal &&
                                          letters[index] != (int) GeneralLetters.Ra2 &&
                                          letters[index] != (int) GeneralLetters.Zeen &&
                                          letters[index] != (int) GeneralLetters.PersianZe &&
                                          letters[index] != (int) GeneralLetters.Alef &&
                                          letters[index] != (int) GeneralLetters.AlefHamza &&
                                          letters[index] != (int) GeneralLetters.AlefMaksoor &&
                                          letters[index] != (int) GeneralLetters.AlefMad &&
                                          letters[index] != (int) GeneralLetters.WawHamza &&
                                          letters[index] != (int) GeneralLetters.Waw &&
                                          letters[index] !=
                                          (int) GeneralLetters.ZeroWidthNoJoiner &&
                                          letters[index] != (int) GeneralLetters.Hamza;

            bool isNextLetterConnectable = index < letters.Count - 1 &&
                                           IsRTLCharacter(letters[index + 1]) &&
                                           letters[index + 1] != (int) GeneralLetters.Hamza &&
                                           letters[index + 1] !=
                                           (int) GeneralLetters.ZeroWidthNoJoiner;

            return isPreviousLetterNonConnectable &&
                   canThisLetterBeLeading &&
                   isNextLetterConnectable;
        }

        /// <summary>
        ///     Is the letter at provided index a finishing letter?
        /// </summary>
        /// <returns><see langword="true" /> if the letter is a finishing letter</returns>
        private static bool IsFinishingLetter(IList<char> letters, int index)
        {
            bool isPreviousLetterConnectable = index != 0 &&
                                               letters[index - 1] != ' ' &&
                                               letters[index - 1] != (int) GeneralLetters.Dal &&
                                               letters[index - 1] != (int) GeneralLetters.Thal &&
                                               letters[index - 1] != (int) GeneralLetters.Ra2 &&
                                               letters[index - 1] != (int) GeneralLetters.Zeen &&
                                               letters[index - 1] != (int) GeneralLetters.PersianZe &&
                                               letters[index - 1] != (int) GeneralLetters.Waw &&
                                               letters[index - 1] != (int) GeneralLetters.Alef &&
                                               letters[index - 1] != (int) GeneralLetters.AlefMad &&
                                               letters[index - 1] != (int) GeneralLetters.AlefHamza &&
                                               letters[index - 1] != (int) GeneralLetters.AlefMaksoor &&
                                               letters[index - 1] != (int) GeneralLetters.WawHamza &&
                                               letters[index - 1] != (int) GeneralLetters.Hamza &&
                                               letters[index - 1] != (int) GeneralLetters.ZeroWidthNoJoiner &&
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


            bool canThisLetterBeFinishing = letters[index] != ' ' &&
                                            letters[index] != (int) GeneralLetters.ZeroWidthNoJoiner &&
                                            letters[index] != (int) GeneralLetters.Hamza;

            return isPreviousLetterConnectable && canThisLetterBeFinishing;
        }

        /// <summary>
        ///     Is the letter at provided index a middle letter?
        /// </summary>
        /// <returns><see langword="true" /> if the letter is a middle letter</returns>
        private static bool IsMiddleLetter(IList<char> letters, int index)
        {
            bool middleLetterCheck = index != 0 &&
                                     letters[index] != (int) GeneralLetters.Alef &&
                                     letters[index] != (int) GeneralLetters.Dal &&
                                     letters[index] != (int) GeneralLetters.Thal &&
                                     letters[index] != (int) GeneralLetters.Ra2 &&
                                     letters[index] != (int) GeneralLetters.Zeen &&
                                     letters[index] != (int) GeneralLetters.PersianZe &&
                                     letters[index] != (int) GeneralLetters.Waw &&
                                     letters[index] != (int) GeneralLetters.AlefMad &&
                                     letters[index] != (int) GeneralLetters.AlefHamza &&
                                     letters[index] != (int) GeneralLetters.AlefMaksoor &&
                                     letters[index] != (int) GeneralLetters.WawHamza &&
                                     letters[index] != (int) GeneralLetters.ZeroWidthNoJoiner &&
                                     letters[index] != (int) GeneralLetters.Hamza;

            bool previousLetterCheck = index != 0 &&
                                       letters[index - 1] != (int) GeneralLetters.Alef &&
                                       letters[index - 1] != (int) GeneralLetters.Dal &&
                                       letters[index - 1] != (int) GeneralLetters.Thal &&
                                       letters[index - 1] != (int) GeneralLetters.Ra2 &&
                                       letters[index - 1] != (int) GeneralLetters.Zeen &&
                                       letters[index - 1] != (int) GeneralLetters.PersianZe &&
                                       letters[index - 1] != (int) GeneralLetters.Waw &&
                                       letters[index - 1] != (int) GeneralLetters.AlefMad &&
                                       letters[index - 1] != (int) GeneralLetters.AlefHamza &&
                                       letters[index - 1] != (int) GeneralLetters.AlefMaksoor &&
                                       letters[index - 1] != (int) GeneralLetters.WawHamza &&
                                       letters[index - 1] != (int) GeneralLetters.Hamza &&
                                       letters[index - 1] !=
                                       (int) GeneralLetters.ZeroWidthNoJoiner &&
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
                                   letters[index + 1] != (int) GeneralLetters.ZeroWidthNoJoiner &&
                                   letters[index + 1] != (int) GeneralLetters.Hamza &&
                                   letters[index + 1] != (int) IsolatedLetters.Hamza;

            return nextLetterCheck && previousLetterCheck && middleLetterCheck;
        }
    }
}