/*
 * This file originally created by: Abdulla Konash. Twitter: @konash
 * Original file can be found here: https://github.com/Konash/arabic-support-unity
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTLTMPro
{
    public static class RTLSupport
    {
        public static string FixRTL(this string input, bool preserveNumbers = true, bool preserveTashkeel = false)
        {
            var tashkeelLocation = new List<TashkeelLocation>();
            string originString = RemoveTashkeel(input, tashkeelLocation);

            char[] lettersOrigin = originString.ToCharArray();
            char[] lettersFinal = originString.ToCharArray();


            for (int i = 0; i < lettersOrigin.Length; i++)
            {
                lettersOrigin[i] = (char)GlyphTable.Convert(lettersOrigin[i]);
            }

            for (int i = 0; i < lettersOrigin.Length; i++)
            {
                bool skip = false;

                // For special Lam Letter connections.
                if (lettersOrigin[i] == (char)IsolatedLetters.Lam)
                {
                    if (i < lettersOrigin.Length - 1)
                    {
                        //lettersOrigin[i + 1] = (char)GlyphTable.ArabicMapper.Convert(lettersOrigin[i + 1]);
                        if (lettersOrigin[i + 1] == (char)IsolatedLetters.AlefMaksoor)
                        {
                            lettersOrigin[i] = (char)0xFEF7;
                            lettersFinal[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if (lettersOrigin[i + 1] == (char)IsolatedLetters.Alef)
                        {
                            lettersOrigin[i] = (char)0xFEF9;
                            lettersFinal[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if (lettersOrigin[i + 1] == (char)IsolatedLetters.AlefHamza)
                        {
                            lettersOrigin[i] = (char)0xFEF5;
                            lettersFinal[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                        else if (lettersOrigin[i + 1] == (char)IsolatedLetters.AlefMad)
                        {
                            lettersOrigin[i] = (char)0xFEF3;
                            lettersFinal[i + 1] = (char)0xFFFF;
                            skip = true;
                        }
                    }
                }


                if (!IsIgnoredCharacter(lettersOrigin[i]))
                {
                    if (IsMiddleLetter(lettersOrigin, i))
                        lettersFinal[i] = (char)(lettersOrigin[i] + 3);
                    else if (IsFinishingLetter(lettersOrigin, i))
                        lettersFinal[i] = (char)(lettersOrigin[i] + 1);
                    else if (IsLeadingLetter(lettersOrigin, i))
                        lettersFinal[i] = (char)(lettersOrigin[i] + 2);
                }

                if (skip)
                    i++;


                //chaning numbers to hindu
                if (!preserveNumbers)
                {
                    if (lettersOrigin[i] == (char)0x0030)
                        lettersFinal[i] = (char)0x0660;
                    else if (lettersOrigin[i] == (char)0x0031)
                        lettersFinal[i] = (char)0x0661;
                    else if (lettersOrigin[i] == (char)0x0032)
                        lettersFinal[i] = (char)0x0662;
                    else if (lettersOrigin[i] == (char)0x0033)
                        lettersFinal[i] = (char)0x0663;
                    else if (lettersOrigin[i] == (char)0x0034)
                        lettersFinal[i] = (char)0x0664;
                    else if (lettersOrigin[i] == (char)0x0035)
                        lettersFinal[i] = (char)0x0665;
                    else if (lettersOrigin[i] == (char)0x0036)
                        lettersFinal[i] = (char)0x0666;
                    else if (lettersOrigin[i] == (char)0x0037)
                        lettersFinal[i] = (char)0x0667;
                    else if (lettersOrigin[i] == (char)0x0038)
                        lettersFinal[i] = (char)0x0668;
                    else if (lettersOrigin[i] == (char)0x0039)
                        lettersFinal[i] = (char)0x0669;
                }
            }


            //Return the Tashkeel to their places.
            if (preserveTashkeel)
                lettersFinal = ReturnTashkeel(lettersFinal, tashkeelLocation);


            List<char> list = new List<char>();

            List<char> numberList = new List<char>();

            for (int i = lettersFinal.Length - 1; i >= 0; i--)
            {
                if (char.IsPunctuation(lettersFinal[i]) && i > 0 && i < lettersFinal.Length - 1 &&
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
                else if (lettersFinal[i] == ' ' && i > 0 && i < lettersFinal.Length - 1 &&
                         (char.IsLower(lettersFinal[i - 1]) || char.IsUpper(lettersFinal[i - 1]) || char.IsNumber(lettersFinal[i - 1])) &&
                         (char.IsLower(lettersFinal[i + 1]) || char.IsUpper(lettersFinal[i + 1]) || char.IsNumber(lettersFinal[i + 1])))

                {
                    numberList.Add(lettersFinal[i]);
                }

                else if (char.IsNumber(lettersFinal[i]) || char.IsLower(lettersFinal[i]) ||
                         char.IsUpper(lettersFinal[i]) || char.IsSymbol(lettersFinal[i]) ||
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
                else if (lettersFinal[i] >= (char)0xD800 && lettersFinal[i] <= (char)0xDBFF ||
                         lettersFinal[i] >= (char)0xDC00 && lettersFinal[i] <= (char)0xDFFF)
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

        public static string FixRTL(this string input, RTLTextMeshPro text, bool preserveNumbers = true, bool preserveTashkeel = false)
        {
            // Populate base text in rect transform and calculate number of lines.
            if (!text.isActiveAndEnabled) return input;

            if (string.IsNullOrEmpty(input))
                return input;

            var finalText = string.Empty;
            var lineEndings = new[] { '\n' };
            foreach (var lines in input.Split(lineEndings))
            {
                var allWords = lines.Split(' ');
                var currentLine = allWords[0].FixRTL(preserveNumbers, preserveTashkeel); // Fix line does character substitution. It doesn't work for multiline
                for (var index = 1; index < allWords.Length; ++index)
                {
                    var previousPreferredHeight = text.preferredHeight;
                    var checkLine = allWords[index].FixRTL(preserveNumbers, preserveTashkeel) + " " + currentLine;
                    text.text = checkLine;
                    text.Rebuild(0);
                    if ((double)text.preferredHeight > previousPreferredHeight)
                    {
                        finalText = finalText + currentLine + "\n";
                        currentLine = allWords[index].FixRTL(preserveNumbers, preserveTashkeel);
                    }
                    else
                        currentLine = allWords[index].FixRTL(preserveNumbers, preserveTashkeel) + " " + currentLine;
                }

                finalText = finalText + currentLine + "\n";
            }

            return finalText.Remove(finalText.Length - 1);
        }

        private static string RemoveTashkeel(string str, ICollection<TashkeelLocation> tashkeelLocation)
        {
            char[] letters = str.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                if (letters[i] == (char)0x064B)
                {
                    // Tanween Fatha
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064B, i));
                }
                else if (letters[i] == (char)0x064C)
                {
                    // Tanween Damma
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064C, i));
                }
                else if (letters[i] == (char)0x064D)
                {
                    // Tanween Kasra
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064D, i));
                }
                else if (letters[i] == (char)0x064E)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064E, i));
                }
                else if (letters[i] == (char)0x064F)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char)0x064F, i));
                }
                else if (letters[i] == (char)0x0650)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0650, i));
                }
                else if (letters[i] == (char)0x0651)
                {
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0651, i));
                }
                else if (letters[i] == (char)0x0652)
                {
                    // SUKUN
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0652, i));
                }
                else if (letters[i] == (char)0x0653)
                {
                    // MADDAH ABOVE
                    tashkeelLocation.Add(new TashkeelLocation((char)0x0653, i));
                }
            }

            string[] split = str.Split((char)0x064B, (char)0x064C, (char)0x064D, (char)0x064E, (char)0x064F, (char)0x0650, (char)0x0651, (char)0x0652, (char)0x0653, (char)0xFC60,
                (char)0xFC61, (char)0xFC62);
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

        /// <summary>
        ///     English letters, numbers and punctuation characters are ignored. This checks if the ch is an ignored character.
        /// </summary>
        /// <param name="ch">The character to be checked for skipping</param>
        /// <returns>True if the character should be ignored, false if it should not be ignored.</returns>
        private static bool IsIgnoredCharacter(char ch)
        {
            bool isPunctuation = char.IsPunctuation(ch);
            bool isNumber = char.IsNumber(ch);
            bool isLower = char.IsLower(ch);
            bool isUpper = char.IsUpper(ch);
            bool isSymbol = char.IsSymbol(ch);
            bool isPersianCharacter = ch == (char)0xFB56 || ch == (char)0xFB7A || ch == (char)0xFB8A || ch == (char)0xFB92 || ch == (char)0xFB8E;
            bool isPresentationFormB = ch <= (char)0xFEFF && ch >= (char)0xFE70;
            bool isAcceptableCharacter = isPresentationFormB || isPersianCharacter || ch == (char)0xFBFC;


            return isPunctuation ||
                   isNumber ||
                   isLower ||
                   isUpper ||
                   isSymbol ||
                   !isAcceptableCharacter ||
                   ch == 'a' || ch == '>' || ch == '<' || ch == (char)0x061B;
        }

        /// <summary>
        ///     Checks if the letter at index value is a leading character in Arabic or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a leading character, else, returns false</returns>
        private static bool IsLeadingLetter(char[] letters, int index)
        {
            bool lettersThatCannotBeBeforeALeadingLetter = index == 0
                                                           || letters[index - 1] == ' '
                                                           || letters[index - 1] == '*' // ??? Remove?
                                                           || letters[index - 1] == 'A' // ??? Remove?
                                                           || char.IsPunctuation(letters[index - 1])
                                                           || letters[index - 1] == '>'
                                                           || letters[index - 1] == '<'
                                                           || letters[index - 1] == (int)IsolatedLetters.Alef
                                                           || letters[index - 1] == (int)IsolatedLetters.Dal
                                                           || letters[index - 1] == (int)IsolatedLetters.Thal
                                                           || letters[index - 1] == (int)IsolatedLetters.Ra2
                                                           || letters[index - 1] == (int)IsolatedLetters.Zeen
                                                           || letters[index - 1] == (int)IsolatedLetters.PersianZe
                                                           || letters[index - 1] == (int)IsolatedLetters.Waw
                                                           || letters[index - 1] == (int)IsolatedLetters.AlefMad
                                                           || letters[index - 1] == (int)IsolatedLetters.AlefHamza
                                                           || letters[index - 1] == (int)IsolatedLetters.Hamza
                                                           || letters[index - 1] == (int)IsolatedLetters.AlefMaksoor
                                                           || letters[index - 1] == (int)IsolatedLetters.WawHamza;

            bool lettersThatCannotBeALeadingLetter = letters[index] != ' '
                                                     && letters[index] != (int)IsolatedLetters.Dal
                                                     && letters[index] != (int)IsolatedLetters.Thal
                                                     && letters[index] != (int)IsolatedLetters.Ra2
                                                     && letters[index] != (int)IsolatedLetters.Zeen
                                                     && letters[index] != (int)IsolatedLetters.PersianZe
                                                     && letters[index] != (int)IsolatedLetters.Alef
                                                     && letters[index] != (int)IsolatedLetters.AlefHamza
                                                     && letters[index] != (int)IsolatedLetters.AlefMaksoor
                                                     && letters[index] != (int)IsolatedLetters.AlefMad
                                                     && letters[index] != (int)IsolatedLetters.WawHamza
                                                     && letters[index] != (int)IsolatedLetters.Waw
                                                     && letters[index] != (int)IsolatedLetters.Hamza;

            bool lettersThatCannotBeAfterLeadingLetter = index < letters.Length - 1
                                                         && letters[index + 1] != ' '
                                                         && !char.IsPunctuation(letters[index + 1])
                                                         && !char.IsNumber(letters[index + 1])
                                                         && !char.IsSymbol(letters[index + 1])
                                                         && !char.IsLower(letters[index + 1])
                                                         && !char.IsUpper(letters[index + 1])
                                                         && letters[index + 1] != (int)IsolatedLetters.Hamza;

            return lettersThatCannotBeBeforeALeadingLetter && lettersThatCannotBeALeadingLetter && lettersThatCannotBeAfterLeadingLetter;
        }

        /// <summary>
        ///     Checks if the letter at index value is a finishing character in Arabic or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a finishing character, else, returns false</returns>
        private static bool IsFinishingLetter(char[] letters, int index)
        {
            bool lettersThatCannotBeBeforeAFinishingLetter = index != 0 && letters[index - 1] != ' ' && letters[index - 1] != (int)IsolatedLetters.Dal &&
                                                             letters[index - 1] != (int)IsolatedLetters.Thal && letters[index - 1] != (int)IsolatedLetters.Ra2 &&
                                                             letters[index - 1] != (int)IsolatedLetters.Zeen && letters[index - 1] != (int)IsolatedLetters.PersianZe &&
                                                             letters[index - 1] != (int)IsolatedLetters.Waw && letters[index - 1] != (int)IsolatedLetters.Alef &&
                                                             letters[index - 1] != (int)IsolatedLetters.AlefMad && letters[index - 1] != (int)IsolatedLetters.AlefHamza &&
                                                             letters[index - 1] != (int)IsolatedLetters.AlefMaksoor && letters[index - 1] != (int)IsolatedLetters.WawHamza &&
                                                             letters[index - 1] != (int)IsolatedLetters.Hamza && !char.IsPunctuation(letters[index - 1]) && !char.IsSymbol(letters[index - 1]) &&
                                                             letters[index - 1] != '>' && letters[index - 1] != '<';


            bool lettersThatCannotBeFinishingLetters = letters[index] != ' ' && letters[index] != (int)IsolatedLetters.Hamza;


            return lettersThatCannotBeBeforeAFinishingLetter && lettersThatCannotBeFinishingLetters;
        }

        /// <summary>
        ///     Checks if the letter at index value is a middle character in Arabic or not.
        /// </summary>
        /// <param name="letters">The whole word that contains the character to be checked</param>
        /// <param name="index">The index of the character to be checked</param>
        /// <returns>True if the character at index is a middle character, else, returns false</returns>
        private static bool IsMiddleLetter(char[] letters, int index)
        {
            bool lettersThatCannotBeMiddleLetters = index != 0 && letters[index] != (int)IsolatedLetters.Alef && letters[index] != (int)IsolatedLetters.Dal &&
                                                    letters[index] != (int)IsolatedLetters.Thal && letters[index] != (int)IsolatedLetters.Ra2 && letters[index] != (int)IsolatedLetters.Zeen &&
                                                    letters[index] != (int)IsolatedLetters.PersianZe && letters[index] != (int)IsolatedLetters.Waw &&
                                                    letters[index] != (int)IsolatedLetters.AlefMad && letters[index] != (int)IsolatedLetters.AlefHamza &&
                                                    letters[index] != (int)IsolatedLetters.AlefMaksoor && letters[index] != (int)IsolatedLetters.WawHamza &&
                                                    letters[index] != (int)IsolatedLetters.Hamza;

            bool lettersThatCannotBeBeforeMiddleCharacters = index != 0 && letters[index - 1] != (int)IsolatedLetters.Alef && letters[index - 1] != (int)IsolatedLetters.Dal &&
                                                             letters[index - 1] != (int)IsolatedLetters.Thal && letters[index - 1] != (int)IsolatedLetters.Ra2 &&
                                                             letters[index - 1] != (int)IsolatedLetters.Zeen && letters[index - 1] != (int)IsolatedLetters.PersianZe &&
                                                             letters[index - 1] != (int)IsolatedLetters.Waw && letters[index - 1] != (int)IsolatedLetters.AlefMad &&
                                                             letters[index - 1] != (int)IsolatedLetters.AlefHamza && letters[index - 1] != (int)IsolatedLetters.AlefMaksoor &&
                                                             letters[index - 1] != (int)IsolatedLetters.WawHamza && letters[index - 1] != (int)IsolatedLetters.Hamza &&
                                                             !char.IsPunctuation(letters[index - 1]) && letters[index - 1] != '>' && letters[index - 1] != '<' && letters[index - 1] != ' ' &&
                                                             letters[index - 1] != '*';

            bool lettersThatCannotBeAfterMiddleCharacters = index < letters.Length - 1 && letters[index + 1] != ' ' && letters[index + 1] != '\r' &&
                                                            letters[index + 1] != (int)IsolatedLetters.Hamza && !char.IsNumber(letters[index + 1]) && !char.IsSymbol(letters[index + 1]) &&
                                                            !char.IsPunctuation(letters[index + 1]);

            if (lettersThatCannotBeAfterMiddleCharacters && lettersThatCannotBeBeforeMiddleCharacters && lettersThatCannotBeMiddleLetters)
            {
                try
                {
                    return !char.IsPunctuation(letters[index + 1]);
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}