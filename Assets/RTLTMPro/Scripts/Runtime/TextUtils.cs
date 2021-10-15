using System;
using System.Collections.Generic;

namespace RTLTMPro
{
    public static class TextUtils
    {
        // Every English character is between these two
        private const char LowerCaseA = (char) 0x61;
        private const char UpperCaseA = (char) 0x41;
        private const char LowerCaseZ = (char) 0x7A;
        private const char UpperCaseZ = (char) 0x5A;

        private const char HebrewLow  = (char) 0x591;
        private const char HebrewHigh = (char) 0x5F4;

        // base, extended and supplement blocks are the isolated forms which will convert to presentation forms by the textbox.
        private const char ArabicBaseBlockLow  = (char) 0x600;
        private const char ArabicBaseBlockHigh = (char) 0x6FF;

        private const char ArabicExtendedABlockLow  = (char)0x8A0;
        private const char ArabicExtendedABlockHigh = (char)0x8FF;

        private const char ArabicExtendedBBlockLow  = (char)0x870;
        private const char ArabicExtendedBBlockHigh = (char)0x89F;

        // presentation forms are final and will be preserved by the textbox
        private const char ArabicPresentationFormsABlockLow  = (char)0xFB50;
        private const char ArabicPresentationFormsABlockHigh = (char)0xFDFF;

        private const char ArabicPresentationFormsBBlockLow  = (char)0xFE70;
        private const char ArabicPresentationFormsBBlockHigh = (char)0xFEFF;

        private static readonly HashSet<char> ArabicLetters;
        static TextUtils() {
            ArabicLetters = new HashSet<char>();
        }

        public static bool IsPunctuation(char ch)
        {
            throw new NotImplementedException();
        }

        public static bool IsNumber(char ch, bool preserverNumbers, bool farsi)
        {
            if (preserverNumbers)
                return IsEnglishNumber(ch);

            if (farsi)
                return IsFarsiNumber(ch);

            return IsHinduNumber(ch);
        }

        public static bool IsEnglishNumber(char ch)
        {
            return ch >= (char) EnglishNumbers.Zero && ch <= (char) EnglishNumbers.Nine;
        }

        public static bool IsFarsiNumber(char ch)
        {
            return ch >= (char) FarsiNumbers.Zero && ch <= (char) FarsiNumbers.Nine;
        }

        public static bool IsHinduNumber(char ch)
        {
            return ch >= (char) HinduNumbers.Zero && ch <= (char) HinduNumbers.Nine;
        }

        public static bool IsEnglishLetter(char ch)
        {
            return ch >= UpperCaseA && ch <= UpperCaseZ || ch >= LowerCaseA && ch <= LowerCaseZ;
        }

        public static bool IsHebrewCharacter(char ch) {
            return ch >= HebrewLow && ch <= HebrewHigh;
        }

        public static bool IsArabicCharacter(char ch) {
            return ch >= ArabicBaseBlockLow && ch <= ArabicBaseBlockHigh
                || ch >= ArabicExtendedABlockLow && ch <= ArabicExtendedABlockHigh
                || ch >= ArabicExtendedBBlockLow && ch <= ArabicExtendedBBlockHigh
                || ch >= ArabicPresentationFormsABlockLow && ch <= ArabicPresentationFormsABlockHigh
                || ch >= ArabicPresentationFormsBBlockLow && ch <= ArabicPresentationFormsBBlockHigh;
        }

        /// <summary>
        ///     Checks if the character is supported RTL character.
        /// </summary>
        /// <param name="ch">Character to check</param>
        /// <returns><see langword="true" /> if character is supported. otherwise <see langword="false" /></returns>
        public static bool IsRTLCharacter(char ch) {
            if (IsHebrewCharacter(ch)) return true;
            if (IsArabicCharacter(ch)) return true;
            return false;
        }

        /// <summary>
        ///     Checks if the character is a known arabic letter which will get transformed by the <see cref="GlyphFixer"/>
        /// </summary>
        /// <param name="ch">Character to check</param>
        /// <returns><see langword="true" /> if character is known and will get glyph fixed. otherwise <see langword="false" /></returns>
        public static bool IsGlyphFixedArabicCharacter(char ch)
        {
            /*
             * Other shapes of each letter comes right after the isolated form.
             * That's why we add 3 to the isolated letter to cover every shape the letter
             */

            if (ch >= (char)ArabicIsolatedLetters.Hamza && ch <= (char)ArabicIsolatedLetters.Hamza + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Alef && ch <= (char)ArabicIsolatedLetters.Alef + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.AlefHamza &&
                ch <= (char)ArabicIsolatedLetters.AlefHamza + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.WawHamza && ch <= (char)ArabicIsolatedLetters.WawHamza + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.AlefMaksoor &&
                ch <= (char)ArabicIsolatedLetters.AlefMaksoor + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.AlefMaksura &&
                ch <= (char)ArabicIsolatedLetters.AlefMaksura + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.HamzaNabera &&
                ch <= (char)ArabicIsolatedLetters.HamzaNabera + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Ba && ch <= (char)ArabicIsolatedLetters.Ba + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Ta && ch <= (char)ArabicIsolatedLetters.Ta + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Tha2 && ch <= (char)ArabicIsolatedLetters.Tha2 + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Jeem && ch <= (char)ArabicIsolatedLetters.Jeem + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.H7aa && ch <= (char)ArabicIsolatedLetters.H7aa + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Khaa2 && ch <= (char)ArabicIsolatedLetters.Khaa2 + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Dal && ch <= (char)ArabicIsolatedLetters.Dal + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Thal && ch <= (char)ArabicIsolatedLetters.Thal + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Ra2 && ch <= (char)ArabicIsolatedLetters.Ra2 + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Zeen && ch <= (char)ArabicIsolatedLetters.Zeen + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Seen && ch <= (char)ArabicIsolatedLetters.Seen + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Sheen && ch <= (char)ArabicIsolatedLetters.Sheen + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.S9a && ch <= (char)ArabicIsolatedLetters.S9a + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Dha && ch <= (char)ArabicIsolatedLetters.Dha + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.T6a && ch <= (char)ArabicIsolatedLetters.T6a + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.T6ha && ch <= (char)ArabicIsolatedLetters.T6ha + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Ain && ch <= (char)ArabicIsolatedLetters.Ain + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Gain && ch <= (char)ArabicIsolatedLetters.Gain + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Fa && ch <= (char)ArabicIsolatedLetters.Fa + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Gaf && ch <= (char)ArabicIsolatedLetters.Gaf + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Kaf && ch <= (char)ArabicIsolatedLetters.Kaf + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Lam && ch <= (char)ArabicIsolatedLetters.Lam + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Meem && ch <= (char)ArabicIsolatedLetters.Meem + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Noon && ch <= (char)ArabicIsolatedLetters.Noon + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Ha && ch <= (char)ArabicIsolatedLetters.Ha + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Waw && ch <= (char)ArabicIsolatedLetters.Waw + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.Ya && ch <= (char)ArabicIsolatedLetters.Ya + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.AlefMad && ch <= (char)ArabicIsolatedLetters.AlefMad + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.TaMarboota &&
                ch <= (char)ArabicIsolatedLetters.TaMarboota + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.PersianPe &&
                ch <= (char)ArabicIsolatedLetters.PersianPe + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.PersianYa &&
                ch <= (char)ArabicIsolatedLetters.PersianYa + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.PersianChe &&
                ch <= (char)ArabicIsolatedLetters.PersianChe + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.PersianZe &&
                ch <= (char)ArabicIsolatedLetters.PersianZe + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.PersianGaf &&
                ch <= (char)ArabicIsolatedLetters.PersianGaf + 3)
            {
                return true;
            }

            if (ch >= (char)ArabicIsolatedLetters.PersianGaf2 &&
                ch <= (char)ArabicIsolatedLetters.PersianGaf2 + 3)
            {
                return true;
            }

            // Special Lam Alef
            if (ch == 0xFEF3)
            {
                return true;
            }

            if (ch == 0xFEF5)
            {
                return true;
            }

            if (ch == 0xFEF7)
            {
                return true;
            }

            if (ch == 0xFEF9)
            {
                return true;
            }

            switch (ch)
            {
                case (char)ArabicGeneralLetters.Hamza:
                case (char)ArabicGeneralLetters.Alef:
                case (char)ArabicGeneralLetters.AlefHamza:
                case (char)ArabicGeneralLetters.WawHamza:
                case (char)ArabicGeneralLetters.AlefMaksoor:
                case (char)ArabicGeneralLetters.AlefMaksura:
                case (char)ArabicGeneralLetters.HamzaNabera:
                case (char)ArabicGeneralLetters.Ba:
                case (char)ArabicGeneralLetters.Ta:
                case (char)ArabicGeneralLetters.Tha2:
                case (char)ArabicGeneralLetters.Jeem:
                case (char)ArabicGeneralLetters.H7aa:
                case (char)ArabicGeneralLetters.Khaa2:
                case (char)ArabicGeneralLetters.Dal:
                case (char)ArabicGeneralLetters.Thal:
                case (char)ArabicGeneralLetters.Ra2:
                case (char)ArabicGeneralLetters.Zeen:
                case (char)ArabicGeneralLetters.Seen:
                case (char)ArabicGeneralLetters.Sheen:
                case (char)ArabicGeneralLetters.S9a:
                case (char)ArabicGeneralLetters.Dha:
                case (char)ArabicGeneralLetters.T6a:
                case (char)ArabicGeneralLetters.T6ha:
                case (char)ArabicGeneralLetters.Ain:
                case (char)ArabicGeneralLetters.Gain:
                case (char)ArabicGeneralLetters.Fa:
                case (char)ArabicGeneralLetters.Gaf:
                case (char)ArabicGeneralLetters.Kaf:
                case (char)ArabicGeneralLetters.Lam:
                case (char)ArabicGeneralLetters.Meem:
                case (char)ArabicGeneralLetters.Noon:
                case (char)ArabicGeneralLetters.Ha:
                case (char)ArabicGeneralLetters.Waw:
                case (char)ArabicGeneralLetters.Ya:
                case (char)ArabicGeneralLetters.AlefMad:
                case (char)ArabicGeneralLetters.TaMarboota:
                case (char)ArabicGeneralLetters.PersianPe:
                case (char)ArabicGeneralLetters.PersianChe:
                case (char)ArabicGeneralLetters.PersianZe:
                case (char)ArabicGeneralLetters.PersianGaf:
                case (char)ArabicGeneralLetters.PersianGaf2:
                case (char)ArabicGeneralLetters.PersianYa:
                case (char)ArabicGeneralLetters.ArabicTatweel:
                case (char)ArabicGeneralLetters.ZeroWidthNoJoiner:
                    return true;
            }

            return false;
        }
        
        /// <summary>
        ///     Checks if the input string starts with supported RTL character or not.
        /// </summary>
        /// <returns><see langword="true" /> if input is RTL. otherwise <see langword="false" /></returns>
        public static bool IsRTLInput(string input)
        {
            bool insideTag = false;
            foreach (char character in input)
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
                    case (char)TashkeelCharacters.Fathan:
                    case (char)TashkeelCharacters.Dammatan:
                    case (char)TashkeelCharacters.Kasratan:
                    case (char)TashkeelCharacters.Fatha:
                    case (char)TashkeelCharacters.Damma:
                    case (char)TashkeelCharacters.Kasra:
                    case (char)TashkeelCharacters.Shadda:
                    case (char)TashkeelCharacters.Sukun:
                    case (char)TashkeelCharacters.MaddahAbove:
                        return true;
                }

                if (insideTag)
                {
                    continue;
                }

                if (char.IsLetter(character))
                {
                    return IsRTLCharacter(character);
                }
            }

            return false;
        }
    }
}