/*
 * This file originally created by: Abdulla Konash. Twitter: @konash
 * Original file can be found here: https://github.com/Konash/arabic-support-unity
 */
 
using System;

namespace RTLTMPro
{
	public static class RTLSupport
	{	
		/// <summary>
		/// Fix the specified string.
		/// </summary>
		/// <param name='str'>
		/// String to be fixed.
		/// </param>
		public static string Fix(string str)
		{
			return Fix(str, false, true);
		}
		
		public static string Fix(string str, bool rtl)
		{
			if(rtl)
			{
				return Fix(str);
			}
			else
			{
				string[] words = str.Split(' ');
				string result = "";
				string arabicToIgnore = "";
				foreach(string word in words)
				{
					if(char.IsLower(word.ToLower()[word.Length/2]))
					{
						result += Fix(arabicToIgnore) + word + " ";
						arabicToIgnore = "";
					}
					else
					{
						arabicToIgnore += word + " ";
						
					}
				}
				if(arabicToIgnore != "")
					result += Fix(arabicToIgnore);
				
				return result;
			}
		}
		
		/// <summary>
		/// Fix the specified string with customization options.
		/// </summary>
		/// <param name='str'>
		/// String to be fixed.
		/// </param>
		/// <param name='showTashkeel'>
		/// Show tashkeel.
		/// </param>
		/// <param name='useHinduNumbers'>
		/// Use hindu numbers.
		/// </param>
		public static string Fix(string str, bool showTashkeel, bool useHinduNumbers)
		{
			RTLFixerTool.showTashkeel = showTashkeel;
			RTLFixerTool.useHinduNumbers =useHinduNumbers;
			
			if(str.Contains("\n"))
				str = str.Replace("\n", Environment.NewLine);
			
			if(str.Contains(Environment.NewLine))
			{
				string[] stringSeparators = new string[] {Environment.NewLine};
				string[] strSplit = str.Split(stringSeparators, StringSplitOptions.None);
				
				if(strSplit.Length == 0)
					return RTLFixerTool.FixLine(str);
				else if(strSplit.Length == 1)
					return RTLFixerTool.FixLine(str);
				else
				{
					string outputString = RTLFixerTool.FixLine(strSplit[0]);
					int iteration = 1;
					if(strSplit.Length > 1)
					{
						while(iteration < strSplit.Length)
						{
							outputString += Environment.NewLine + RTLFixerTool.FixLine(strSplit[iteration]);
							iteration++;
						}
					}				
					return outputString;
				}	
			}
			else
			{
				return RTLFixerTool.FixLine(str);
			}
			
		}

        public static string Fix(string str, bool showTashkeel, bool combineTashkeel, bool useHinduNumbers)
        {
            RTLFixerTool.combineTashkeel = combineTashkeel;
            return Fix(str, showTashkeel, useHinduNumbers);
        }


    }
	
}