using System;

namespace RTLTMPro
{
    public static class TextUtils
    {
        // Every English character is between these two
        private const char UpperCaseA = (char) 0x41;
        private const char LowerCaseZ = (char) 0x7A;
        
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
            return ch >= UpperCaseA && ch <= LowerCaseZ;
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

            if (ch >= (char)IsolatedLetters.Hamza && ch <= (char)IsolatedLetters.Hamza + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Alef && ch <= (char)IsolatedLetters.Alef + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.AlefHamza &&
                ch <= (char)IsolatedLetters.AlefHamza + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.WawHamza && ch <= (char)IsolatedLetters.WawHamza + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.AlefMaksoor &&
                ch <= (char)IsolatedLetters.AlefMaksoor + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.AlefMaksura &&
                ch <= (char)IsolatedLetters.AlefMaksura + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.HamzaNabera &&
                ch <= (char)IsolatedLetters.HamzaNabera + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Ba && ch <= (char)IsolatedLetters.Ba + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Ta && ch <= (char)IsolatedLetters.Ta + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Tha2 && ch <= (char)IsolatedLetters.Tha2 + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Jeem && ch <= (char)IsolatedLetters.Jeem + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.H7aa && ch <= (char)IsolatedLetters.H7aa + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Khaa2 && ch <= (char)IsolatedLetters.Khaa2 + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Dal && ch <= (char)IsolatedLetters.Dal + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Thal && ch <= (char)IsolatedLetters.Thal + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Ra2 && ch <= (char)IsolatedLetters.Ra2 + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Zeen && ch <= (char)IsolatedLetters.Zeen + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Seen && ch <= (char)IsolatedLetters.Seen + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Sheen && ch <= (char)IsolatedLetters.Sheen + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.S9a && ch <= (char)IsolatedLetters.S9a + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Dha && ch <= (char)IsolatedLetters.Dha + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.T6a && ch <= (char)IsolatedLetters.T6a + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.T6ha && ch <= (char)IsolatedLetters.T6ha + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Ain && ch <= (char)IsolatedLetters.Ain + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Gain && ch <= (char)IsolatedLetters.Gain + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Fa && ch <= (char)IsolatedLetters.Fa + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Gaf && ch <= (char)IsolatedLetters.Gaf + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Kaf && ch <= (char)IsolatedLetters.Kaf + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Lam && ch <= (char)IsolatedLetters.Lam + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Meem && ch <= (char)IsolatedLetters.Meem + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Noon && ch <= (char)IsolatedLetters.Noon + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Ha && ch <= (char)IsolatedLetters.Ha + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Waw && ch <= (char)IsolatedLetters.Waw + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.Ya && ch <= (char)IsolatedLetters.Ya + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.AlefMad && ch <= (char)IsolatedLetters.AlefMad + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.TaMarboota &&
                ch <= (char)IsolatedLetters.TaMarboota + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.PersianPe &&
                ch <= (char)IsolatedLetters.PersianPe + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.PersianYa &&
                ch <= (char)IsolatedLetters.PersianYa + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.PersianChe &&
                ch <= (char)IsolatedLetters.PersianChe + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.PersianZe &&
                ch <= (char)IsolatedLetters.PersianZe + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.PersianGaf &&
                ch <= (char)IsolatedLetters.PersianGaf + 3)
            {
                return true;
            }

            if (ch >= (char)IsolatedLetters.PersianGaf2 &&
                ch <= (char)IsolatedLetters.PersianGaf2 + 3)
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
                case (char)GeneralLetters.Hamza:
                case (char)GeneralLetters.Alef:
                case (char)GeneralLetters.AlefHamza:
                case (char)GeneralLetters.WawHamza:
                case (char)GeneralLetters.AlefMaksoor:
                case (char)GeneralLetters.AlefMaksura:
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
                case (char)GeneralLetters.PersianYa:
                case (char)GeneralLetters.ArabicTatweel:
                case (char)GeneralLetters.ZeroWidthNoJoiner:
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