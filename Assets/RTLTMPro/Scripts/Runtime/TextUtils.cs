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

        public static bool IsArabicPresentationForm(char ch)
        {
            return ch >= ArabicPresentationFormsABlockLow && ch <= ArabicPresentationFormsABlockHigh
                || ch >= ArabicPresentationFormsBBlockLow && ch <= ArabicPresentationFormsBBlockHigh;
        }

        public static bool IsPunctuation(char ch)
        {
            throw new NotImplementedException();
        }

        public static bool IsNumber(char ch, bool preserveNumbers, bool farsi)
        {
            if (preserveNumbers)
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
            switch (ch)
            {
                case (char)ArabicGeneralLetters.Hamza:
                case (char)ArabicGeneralLetters.AlefMaddaAbove:
                case (char)ArabicGeneralLetters.AlefHamzaAbove:
                case (char)ArabicGeneralLetters.WawHamzaAbove:
                case (char)ArabicGeneralLetters.AlefHamzaBelow:
                case (char)ArabicGeneralLetters.YehHamzaAbove:
                case (char)ArabicGeneralLetters.Alef:
                case (char)ArabicGeneralLetters.Beh:
                case (char)ArabicGeneralLetters.TehMarbuta:
                case (char)ArabicGeneralLetters.Teh:
                case (char)ArabicGeneralLetters.Theh:
                case (char)ArabicGeneralLetters.Jeem:
                case (char)ArabicGeneralLetters.Hah:
                case (char)ArabicGeneralLetters.Khah:
                case (char)ArabicGeneralLetters.Dal:
                case (char)ArabicGeneralLetters.Thal:
                case (char)ArabicGeneralLetters.Reh:
                case (char)ArabicGeneralLetters.Zain:
                case (char)ArabicGeneralLetters.Seen:
                case (char)ArabicGeneralLetters.Sheen:
                case (char)ArabicGeneralLetters.Sad:
                case (char)ArabicGeneralLetters.Dad:
                case (char)ArabicGeneralLetters.Tah:
                case (char)ArabicGeneralLetters.Zah:
                case (char)ArabicGeneralLetters.Ain:
                case (char)ArabicGeneralLetters.Ghain:
                case (char)ArabicGeneralLetters.Feh:
                case (char)ArabicGeneralLetters.Qaf:
                case (char)ArabicGeneralLetters.Kaf:
                case (char)ArabicGeneralLetters.Lam:
                case (char)ArabicGeneralLetters.Meem:
                case (char)ArabicGeneralLetters.Noon:
                case (char)ArabicGeneralLetters.Heh:
                case (char)ArabicGeneralLetters.Waw:
                case (char)ArabicGeneralLetters.AlefMaksura:
                case (char)ArabicGeneralLetters.Yeh:
                case (char)ArabicGeneralLetters.FarsiYeh:
                case (char)ArabicGeneralLetters.Peh:
                case (char)ArabicGeneralLetters.TCheh:
                case (char)ArabicGeneralLetters.Jeh:
                case (char)ArabicGeneralLetters.Keheh:
                case (char)ArabicGeneralLetters.Gaf:
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