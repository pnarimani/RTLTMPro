using System;
using System.Collections.Generic;

namespace RTLTMPro
{
    /// <summary>
    ///     Sets up and creates the conversion table
    /// </summary>
    public static class GlyphTable
    {
        private static readonly Dictionary<char,char> MapList;

        /// <summary>
        ///     Setting up the conversion table
        /// </summary>
        static GlyphTable()
        { 
            //using GetNames instead of GetValues to be able to match enums
            var isolatedValues = Enum.GetNames(typeof(IsolatedLetters));
            
            MapList = new Dictionary<char,char>(isolatedValues.Length);
            foreach (var value in isolatedValues)
                MapList.Add((char)(int) Enum.Parse(typeof(GeneralLetters),value), (char) (int)Enum.Parse(typeof(IsolatedLetters),value));
        }

        public static char Convert(char toBeConverted)
        {
            char convertedValue;
            return MapList.TryGetValue(toBeConverted, out convertedValue) ? convertedValue : toBeConverted;
        }
    }
}