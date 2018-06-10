using System;
using System.Collections.Generic;

namespace RTLTMPro
{
    public static class RTLFixerTool
    {
        internal static bool showTashkeel = true;
        internal static bool combineTashkeel = true;
        internal static bool useHinduNumbers = false;
	
	
	internal static string RemoveTashkeel(string str, out List<TashkeelLocation> tashkeelLocation)
	{
		tashkeelLocation = new List<TashkeelLocation>();
		char[] letters = str.ToCharArray();

		int index = 0;
		for (int i = 0; i < letters.Length; i++) {
			if (letters [i] == (char)0x064B) { // Tanween Fatha
				tashkeelLocation.Add (new TashkeelLocation ((char)0x064B, i));
				index++;
			}
            else if (letters [i] == (char)0x064C) { // Tanween Damma
				tashkeelLocation.Add (new TashkeelLocation ((char)0x064C, i));
				index++;
			}
            else if (letters [i] == (char)0x064D){ // Tanween Kasra
				tashkeelLocation.Add (new TashkeelLocation ((char)0x064D, i));
				index++;
			}
            else if (letters [i] == (char)0x064E) { // Fatha
				if(index > 0 && combineTashkeel)
				{
					if(tashkeelLocation[index-1].tashkeel == (char)0x0651 ) // Shadda
					{
						tashkeelLocation [index - 1].tashkeel = (char)0xFC60; // Shadda With Fatha
						continue;
					}
				}

				tashkeelLocation.Add (new TashkeelLocation ((char)0x064E, i));
				index++;
			}
            else if (letters [i] == (char)0x064F) { // DAMMA
				if (index > 0 && combineTashkeel) {
					if (tashkeelLocation [index - 1].tashkeel == (char)0x0651) { // SHADDA
						tashkeelLocation [index - 1].tashkeel = (char)0xFC61; // Shadda With DAMMA
						continue;
					}
				}
				tashkeelLocation.Add (new TashkeelLocation ((char)0x064F, i));
				index++;
			}
            else if (letters [i] == (char)0x0650) { // KASRA
				if (index > 0 && combineTashkeel) {
					if (tashkeelLocation [index - 1].tashkeel == (char)0x0651) { // SHADDA
						tashkeelLocation [index - 1].tashkeel = (char)0xFC62; // Shadda With KASRA
						continue;
					}
				}
				tashkeelLocation.Add (new TashkeelLocation ((char)0x0650, i));
				index++;
			}
            else if (letters [i] == (char)0x0651) { // SHADDA
				if(index > 0 && combineTashkeel)
				{
					if(tashkeelLocation[index-1].tashkeel == (char)0x064E ) // FATHA
					{
						tashkeelLocation [index - 1].tashkeel = (char)0xFC60; // Shadda With Fatha
						continue;
					}

					if(tashkeelLocation[index-1].tashkeel == (char)0x064F ) // DAMMA
					{
						tashkeelLocation [index - 1].tashkeel = (char)0xFC61; // Shadda With DAMMA
						continue;
					}

					if(tashkeelLocation[index-1].tashkeel == (char)0x0650 ) // KASRA
					{
						tashkeelLocation [index - 1].tashkeel = (char)0xFC62; // Shadda With KASRA
						continue;
					}
				}

				tashkeelLocation.Add (new TashkeelLocation ((char)0x0651, i));
				index++;
			}
            else if (letters [i] == (char)0x0652) { // SUKUN
				tashkeelLocation.Add (new TashkeelLocation ((char)0x0652, i));
				index++;
			}
            else if (letters [i] == (char)0x0653) { // MADDAH ABOVE
				tashkeelLocation.Add (new TashkeelLocation ((char)0x0653, i));
				index++;
			}
		}
		
		string[] split = str.Split(new char[]{(char)0x064B,(char)0x064C,(char)0x064D,
			(char)0x064E,(char)0x064F,(char)0x0650,
		
			(char)0x0651,(char)0x0652,(char)0x0653,(char)0xFC60,(char)0xFC61,(char)0xFC62});
		str = "";
		
		foreach(string s in split)
		{
			str += s;
		}
		
		return str;
	}
	
	internal static char[] ReturnTashkeel(char[] letters, List<TashkeelLocation> tashkeelLocation)
	{
		char[] lettersWithTashkeel = new char[letters.Length + tashkeelLocation.Count];
		
		int letterWithTashkeelTracker = 0;
		for(int i = 0; i<letters.Length; i++)
		{
			lettersWithTashkeel[letterWithTashkeelTracker] = letters[i];
			letterWithTashkeelTracker++;
			foreach(TashkeelLocation hLocation in tashkeelLocation)
			{
				if(hLocation.position == letterWithTashkeelTracker)
				{
					lettersWithTashkeel[letterWithTashkeelTracker] = hLocation.tashkeel;
					letterWithTashkeelTracker++;
				}
			}
		}
		
		return lettersWithTashkeel;
	}
	
	/// <summary>
	/// Converts a string to a form in which the sting will be displayed correctly for arabic text.
	/// </summary>
	/// <param name="str">String to be converted. Example: "Aaa"</param>
	/// <returns>Converted string. Example: "aa aaa A" without the spaces.</returns>
	internal static string FixLine(string str)
	{
		string test = "";
		
		List<TashkeelLocation> tashkeelLocation;
		
		string originString = RemoveTashkeel(str, out tashkeelLocation);
		
		char[] lettersOrigin = originString.ToCharArray();
		char[] lettersFinal = originString.ToCharArray();
		

		
		for (int i = 0; i < lettersOrigin.Length; i++)
		{
			lettersOrigin[i] = (char)GlyphTable.Convert(lettersOrigin[i]);
		}
		
		for (int i = 0; i < lettersOrigin.Length; i++)
		{
			bool skip = false;

			
			//lettersOrigin[i] = (char)GlyphTable.ArabicMapper.Convert(lettersOrigin[i]);


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

            //string strOut = String.Format(@"\x{0:x4}", (ushort)lettersOrigin[i]);
            //UnityEngine.Debug.Log(strOut);

            //strOut = String.Format(@"\x{0:x4}", (ushort)lettersFinal[i]);
            //UnityEngine.Debug.Log(strOut);

            test += Convert.ToString((int)lettersOrigin[i], 16) + " ";
			if (skip)
				i++;
			
			
			//chaning numbers to hindu
			if(useHinduNumbers){
				if(lettersOrigin[i] == (char)0x0030)
					lettersFinal[i] = (char)0x0660;
				else if(lettersOrigin[i] == (char)0x0031)
					lettersFinal[i] = (char)0x0661;
				else if(lettersOrigin[i] == (char)0x0032)
					lettersFinal[i] = (char)0x0662;
				else if(lettersOrigin[i] == (char)0x0033)
					lettersFinal[i] = (char)0x0663;
				else if(lettersOrigin[i] == (char)0x0034)
					lettersFinal[i] = (char)0x0664;
				else if(lettersOrigin[i] == (char)0x0035)
					lettersFinal[i] = (char)0x0665;
				else if(lettersOrigin[i] == (char)0x0036)
					lettersFinal[i] = (char)0x0666;
				else if(lettersOrigin[i] == (char)0x0037)
					lettersFinal[i] = (char)0x0667;
				else if(lettersOrigin[i] == (char)0x0038)
					lettersFinal[i] = (char)0x0668;
				else if(lettersOrigin[i] == (char)0x0039)
					lettersFinal[i] = (char)0x0669;
			}
			
		}
		
		
		
		//Return the Tashkeel to their places.
		if(showTashkeel)
			lettersFinal = ReturnTashkeel(lettersFinal, tashkeelLocation);
		
		
		List<char> list = new List<char>();
		
		List<char> numberList = new List<char>();
		
		for (int i = lettersFinal.Length - 1; i >= 0; i--)
		{
			
			
			//				if (lettersFinal[i] == '(')
			//						numberList.Add(')');
			//				else if (lettersFinal[i] == ')')
			//					numberList.Add('(');
			//				else if (lettersFinal[i] == '<')
			//					numberList.Add('>');
			//				else if (lettersFinal[i] == '>')
			//					numberList.Add('<');
			//				else 
			if (char.IsPunctuation(lettersFinal[i]) && i>0 && i < lettersFinal.Length-1 &&
			    (char.IsPunctuation(lettersFinal[i-1]) || char.IsPunctuation(lettersFinal[i+1])))
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
			else if(lettersFinal[i] == ' ' && i > 0 && i < lettersFinal.Length-1 &&
			        (char.IsLower(lettersFinal[i-1]) || char.IsUpper(lettersFinal[i-1]) || char.IsNumber(lettersFinal[i-1])) &&
			        (char.IsLower(lettersFinal[i+1]) || char.IsUpper(lettersFinal[i+1]) ||char.IsNumber(lettersFinal[i+1])))
				
			{
				numberList.Add(lettersFinal[i]);
			}
			
			else if (char.IsNumber(lettersFinal[i]) || char.IsLower(lettersFinal[i]) ||
			         char.IsUpper(lettersFinal[i]) || char.IsSymbol(lettersFinal[i]) ||
			         char.IsPunctuation(lettersFinal[i]))// || lettersFinal[i] == '^') //)
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
			else if( lettersFinal[i] >= (char)0xD800 && lettersFinal[i] <= (char)0xDBFF ||
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
		
		
		str = new string(lettersFinal);
		return str;
	}
	
	/// <summary>
	/// English letters, numbers and punctuation characters are ignored. This checks if the ch is an ignored character.
	/// </summary>
	/// <param name="ch">The character to be checked for skipping</param>
	/// <returns>True if the character should be ignored, false if it should not be ignored.</returns>
	internal static bool IsIgnoredCharacter(char ch)
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
		
		//            return char.IsPunctuation(ch) || char.IsNumber(ch) || ch == 'a' || ch == '>' || ch == '<' ||
		//                    char.IsLower(ch) || char.IsUpper(ch) || ch == (char)0x061B || char.IsSymbol(ch)
		//					|| !(ch <= (char)0xFEFF && ch >= (char)0xFE70) // Presentation Form B
		//					|| ch == (char)0xFB56 || ch == (char)0xFB7A || ch == (char)0xFB8A || ch == (char)0xFB92; // Persian Characters
		
		//					PersianPe = 0xFB56,
		//		PersianChe = 0xFB7A,
		//		PersianZe = 0xFB8A,
		//		PersianGaf = 0xFB92
		//lettersOrigin[i] <= (char)0xFEFF && lettersOrigin[i] >= (char)0xFE70
	}
	
	/// <summary>
	/// Checks if the letter at index value is a leading character in Arabic or not.
	/// </summary>
	/// <param name="letters">The whole word that contains the character to be checked</param>
	/// <param name="index">The index of the character to be checked</param>
	/// <returns>True if the character at index is a leading character, else, returns false</returns>
	internal static bool IsLeadingLetter(char[] letters, int index)
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
				//|| letters[index - 1] == (int)IsolatedLetters.AlefMaksora 
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
				&& !char.IsPunctuation(letters[index + 1] )
				&& !char.IsNumber(letters[index + 1])
				&& !char.IsSymbol(letters[index + 1])
				&& !char.IsLower(letters[index + 1])
				&& !char.IsUpper(letters[index + 1])
				&& letters[index + 1] != (int)IsolatedLetters.Hamza;

		if(lettersThatCannotBeBeforeALeadingLetter && lettersThatCannotBeALeadingLetter && lettersThatCannotBeAfterLeadingLetter)

//		if ((index == 0 || letters[index - 1] == ' ' || letters[index - 1] == '*' || letters[index - 1] == 'A' || char.IsPunctuation(letters[index - 1])
//		     || letters[index - 1] == '>' || letters[index - 1] == '<' 
//		     || letters[index - 1] == (int)IsolatedLetters.Alef
//		     || letters[index - 1] == (int)IsolatedLetters.Dal || letters[index - 1] == (int)IsolatedLetters.Thal
//		     || letters[index - 1] == (int)IsolatedLetters.Ra2 
//		     || letters[index - 1] == (int)IsolatedLetters.Zeen || letters[index - 1] == (int)IsolatedLetters.PersianZe
//		     || letters[index - 1] == (int)IsolatedLetters.AlefMaksora || letters[index - 1] == (int)IsolatedLetters.Waw
//		     || letters[index - 1] == (int)IsolatedLetters.AlefMad || letters[index - 1] == (int)IsolatedLetters.AlefHamza
//		     || letters[index - 1] == (int)IsolatedLetters.AlefMaksoor || letters[index - 1] == (int)IsolatedLetters.WawHamza) 
//		    && letters[index] != ' ' && letters[index] != (int)IsolatedLetters.Dal
//		    && letters[index] != (int)IsolatedLetters.Thal
//		    && letters[index] != (int)IsolatedLetters.Ra2 
//		    && letters[index] != (int)IsolatedLetters.Zeen && letters[index] != (int)IsolatedLetters.PersianZe
//		    && letters[index] != (int)IsolatedLetters.Alef && letters[index] != (int)IsolatedLetters.AlefHamza
//		    && letters[index] != (int)IsolatedLetters.AlefMaksoor
//		    && letters[index] != (int)IsolatedLetters.AlefMad
//		    && letters[index] != (int)IsolatedLetters.WawHamza
//		    && letters[index] != (int)IsolatedLetters.Waw
//		    && letters[index] != (int)IsolatedLetters.Hamza
//		    && index < letters.Length - 1 && letters[index + 1] != ' ' && !char.IsPunctuation(letters[index + 1] ) && !char.IsNumber(letters[index + 1])
//		    && letters[index + 1] != (int)IsolatedLetters.Hamza )
		{
			return true;
		}
		else
			return false;
	}
	
	/// <summary>
	/// Checks if the letter at index value is a finishing character in Arabic or not.
	/// </summary>
	/// <param name="letters">The whole word that contains the character to be checked</param>
	/// <param name="index">The index of the character to be checked</param>
	/// <returns>True if the character at index is a finishing character, else, returns false</returns>
	internal static bool IsFinishingLetter(char[] letters, int index)
	{
		bool indexZero = index != 0;
		bool lettersThatCannotBeBeforeAFinishingLetter = index == 0 ? false : 
				letters[index - 1] != ' '
//				&& char.IsDigit(letters[index-1])
//				&& char.IsLower(letters[index-1])
//				&& char.IsUpper(letters[index-1])
//				&& char.IsNumber(letters[index-1])
//				&& char.IsWhiteSpace(letters[index-1])
//				&& char.IsPunctuation(letters[index-1])
//				&& char.IsSymbol(letters[index-1])

				&& letters[index - 1] != (int)IsolatedLetters.Dal 
				&& letters[index - 1] != (int)IsolatedLetters.Thal
				&& letters[index - 1] != (int)IsolatedLetters.Ra2 
				&& letters[index - 1] != (int)IsolatedLetters.Zeen 
				&& letters[index - 1] != (int)IsolatedLetters.PersianZe
				//&& letters[index - 1] != (int)IsolatedLetters.AlefMaksora 
				&& letters[index - 1] != (int)IsolatedLetters.Waw
				&& letters[index - 1] != (int)IsolatedLetters.Alef 
				&& letters[index - 1] != (int)IsolatedLetters.AlefMad
				&& letters[index - 1] != (int)IsolatedLetters.AlefHamza 
				&& letters[index - 1] != (int)IsolatedLetters.AlefMaksoor
				&& letters[index - 1] != (int)IsolatedLetters.WawHamza 
				&& letters[index - 1] != (int)IsolatedLetters.Hamza



				&& !char.IsPunctuation(letters[index - 1]) 
                && !char.IsSymbol(letters[index-1])
				&& letters[index - 1] != '>' 
				&& letters[index - 1] != '<';
				

		bool lettersThatCannotBeFinishingLetters = letters[index] != ' ' && letters[index] != (int)IsolatedLetters.Hamza;

	


		if(lettersThatCannotBeBeforeAFinishingLetter && lettersThatCannotBeFinishingLetters)

//		if (index != 0 && letters[index - 1] != ' ' && letters[index - 1] != '*' && letters[index - 1] != 'A'
//		    && letters[index - 1] != (int)IsolatedLetters.Dal && letters[index - 1] != (int)IsolatedLetters.Thal
//		    && letters[index - 1] != (int)IsolatedLetters.Ra2 
//		    && letters[index - 1] != (int)IsolatedLetters.Zeen && letters[index - 1] != (int)IsolatedLetters.PersianZe
//		    && letters[index - 1] != (int)IsolatedLetters.AlefMaksora && letters[index - 1] != (int)IsolatedLetters.Waw
//		    && letters[index - 1] != (int)IsolatedLetters.Alef && letters[index - 1] != (int)IsolatedLetters.AlefMad
//		    && letters[index - 1] != (int)IsolatedLetters.AlefHamza && letters[index - 1] != (int)IsolatedLetters.AlefMaksoor
//		    && letters[index - 1] != (int)IsolatedLetters.WawHamza && letters[index - 1] != (int)IsolatedLetters.Hamza 
//		    && !char.IsPunctuation(letters[index - 1]) && letters[index - 1] != '>' && letters[index - 1] != '<' 
//		    && letters[index] != ' ' && index < letters.Length
//		    && letters[index] != (int)IsolatedLetters.Hamza)
		{
			//try
			//{
			//    if (char.IsPunctuation(letters[index + 1]))
			//        return true;
			//    else
			//        return false;
			//}
			//catch (Exception e)
			//{
			//    return false;
			//}
			
			return true;
		}
		//return true;
		else
			return false;
	}
	
	/// <summary>
	/// Checks if the letter at index value is a middle character in Arabic or not.
	/// </summary>
	/// <param name="letters">The whole word that contains the character to be checked</param>
	/// <param name="index">The index of the character to be checked</param>
	/// <returns>True if the character at index is a middle character, else, returns false</returns>
	internal static bool IsMiddleLetter(char[] letters, int index)
	{
		bool lettersThatCannotBeMiddleLetters = index == 0 ? false : 
			letters[index] != (int)IsolatedLetters.Alef 
				&& letters[index] != (int)IsolatedLetters.Dal
				&& letters[index] != (int)IsolatedLetters.Thal 
				&& letters[index] != (int)IsolatedLetters.Ra2
				&& letters[index] != (int)IsolatedLetters.Zeen 
				&& letters[index] != (int)IsolatedLetters.PersianZe 
				//&& letters[index] != (int)IsolatedLetters.AlefMaksora
				&& letters[index] != (int)IsolatedLetters.Waw 
				&& letters[index] != (int)IsolatedLetters.AlefMad
				&& letters[index] != (int)IsolatedLetters.AlefHamza 
				&& letters[index] != (int)IsolatedLetters.AlefMaksoor
				&& letters[index] != (int)IsolatedLetters.WawHamza 
				&& letters[index] != (int)IsolatedLetters.Hamza;

		bool lettersThatCannotBeBeforeMiddleCharacters = index == 0 ? false :
				letters[index - 1] != (int)IsolatedLetters.Alef 
				&& letters[index - 1] != (int)IsolatedLetters.Dal
				&& letters[index - 1] != (int)IsolatedLetters.Thal 
				&& letters[index - 1] != (int)IsolatedLetters.Ra2
				&& letters[index - 1] != (int)IsolatedLetters.Zeen 
				&& letters[index - 1] != (int)IsolatedLetters.PersianZe 
				//&& letters[index - 1] != (int)IsolatedLetters.AlefMaksora
				&& letters[index - 1] != (int)IsolatedLetters.Waw 
				&& letters[index - 1] != (int)IsolatedLetters.AlefMad
				&& letters[index - 1] != (int)IsolatedLetters.AlefHamza 
				&& letters[index - 1] != (int)IsolatedLetters.AlefMaksoor
				&& letters[index - 1] != (int)IsolatedLetters.WawHamza 
				&& letters[index - 1] != (int)IsolatedLetters.Hamza
				&& !char.IsPunctuation(letters[index - 1])
				&& letters[index - 1] != '>' 
				&& letters[index - 1] != '<' 
				&& letters[index - 1] != ' ' 
				&& letters[index - 1] != '*';

		bool lettersThatCannotBeAfterMiddleCharacters = index >= letters.Length - 1 ? false :
			letters[index + 1] != ' ' 
				&& letters[index + 1] != '\r' 
				&& letters[index + 1] != (int)IsolatedLetters.Hamza
				&& !char.IsNumber(letters[index + 1])
				&& !char.IsSymbol(letters[index + 1])
				&& !char.IsPunctuation(letters[index + 1]);
		if(lettersThatCannotBeAfterMiddleCharacters && lettersThatCannotBeBeforeMiddleCharacters && lettersThatCannotBeMiddleLetters)

//		if (index != 0 && letters[index] != ' '
//		    && letters[index] != (int)IsolatedLetters.Alef && letters[index] != (int)IsolatedLetters.Dal
//		    && letters[index] != (int)IsolatedLetters.Thal && letters[index] != (int)IsolatedLetters.Ra2
//		    && letters[index] != (int)IsolatedLetters.Zeen && letters[index] != (int)IsolatedLetters.PersianZe 
//		    && letters[index] != (int)IsolatedLetters.AlefMaksora
//		    && letters[index] != (int)IsolatedLetters.Waw && letters[index] != (int)IsolatedLetters.AlefMad
//		    && letters[index] != (int)IsolatedLetters.AlefHamza && letters[index] != (int)IsolatedLetters.AlefMaksoor
//		    && letters[index] != (int)IsolatedLetters.WawHamza && letters[index] != (int)IsolatedLetters.Hamza
//		    && letters[index - 1] != (int)IsolatedLetters.Alef && letters[index - 1] != (int)IsolatedLetters.Dal
//		    && letters[index - 1] != (int)IsolatedLetters.Thal && letters[index - 1] != (int)IsolatedLetters.Ra2
//		    && letters[index - 1] != (int)IsolatedLetters.Zeen && letters[index - 1] != (int)IsolatedLetters.PersianZe 
//		    && letters[index - 1] != (int)IsolatedLetters.AlefMaksora
//		    && letters[index - 1] != (int)IsolatedLetters.Waw && letters[index - 1] != (int)IsolatedLetters.AlefMad
//		    && letters[index - 1] != (int)IsolatedLetters.AlefHamza && letters[index - 1] != (int)IsolatedLetters.AlefMaksoor
//		    && letters[index - 1] != (int)IsolatedLetters.WawHamza && letters[index - 1] != (int)IsolatedLetters.Hamza 
//		    && letters[index - 1] != '>' && letters[index - 1] != '<' 
//		    && letters[index - 1] != ' ' && letters[index - 1] != '*' && !char.IsPunctuation(letters[index - 1])
//		    && index < letters.Length - 1 && letters[index + 1] != ' ' && letters[index + 1] != '\r' && letters[index + 1] != 'A' 
//		    && letters[index + 1] != '>' && letters[index + 1] != '>' && letters[index + 1] != (int)IsolatedLetters.Hamza
//		    )
		{
			try
			{
				if (char.IsPunctuation(letters[index + 1]))
					return false;
				else
					return true;
			}
			catch
			{
				return false;
			}
			//return true;
		}
		else
			return false;
	}
	
    }
}