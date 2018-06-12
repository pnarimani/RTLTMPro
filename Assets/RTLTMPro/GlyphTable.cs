using System.Collections.Generic;

namespace RTLTMPro
{
    /// <summary>
    ///     Sets up and creates the conversion table
    /// </summary>
    public static class GlyphTable
    {
        private static readonly List<GlyphMapping> MapList;

        /// <summary>
        ///     Setting up the conversion table
        /// </summary>
        static GlyphTable()
        {
            MapList = new List<GlyphMapping>
            {
                new GlyphMapping((int) GeneralLetters.Hamza, (int) IsolatedLetters.Hamza),
                new GlyphMapping((int) GeneralLetters.Alef, (int) IsolatedLetters.Alef),
                new GlyphMapping((int) GeneralLetters.AlefHamza, (int) IsolatedLetters.AlefHamza),
                new GlyphMapping((int) GeneralLetters.WawHamza, (int) IsolatedLetters.WawHamza),
                new GlyphMapping((int) GeneralLetters.AlefMaksoor, (int) IsolatedLetters.AlefMaksoor),
                new GlyphMapping((int) GeneralLetters.AlefMagsora, (int) IsolatedLetters.AlefMaksora),
                new GlyphMapping((int) GeneralLetters.HamzaNabera, (int) IsolatedLetters.HamzaNabera),
                new GlyphMapping((int) GeneralLetters.Ba, (int) IsolatedLetters.Ba),
                new GlyphMapping((int) GeneralLetters.Ta, (int) IsolatedLetters.Ta),
                new GlyphMapping((int) GeneralLetters.Tha2, (int) IsolatedLetters.Tha2),
                new GlyphMapping((int) GeneralLetters.Jeem, (int) IsolatedLetters.Jeem),
                new GlyphMapping((int) GeneralLetters.H7aa, (int) IsolatedLetters.H7aa),
                new GlyphMapping((int) GeneralLetters.Khaa2, (int) IsolatedLetters.Khaa2),
                new GlyphMapping((int) GeneralLetters.Dal, (int) IsolatedLetters.Dal),
                new GlyphMapping((int) GeneralLetters.Thal, (int) IsolatedLetters.Thal),
                new GlyphMapping((int) GeneralLetters.Ra2, (int) IsolatedLetters.Ra2),
                new GlyphMapping((int) GeneralLetters.Zeen, (int) IsolatedLetters.Zeen),
                new GlyphMapping((int) GeneralLetters.Seen, (int) IsolatedLetters.Seen),
                new GlyphMapping((int) GeneralLetters.Sheen, (int) IsolatedLetters.Sheen),
                new GlyphMapping((int) GeneralLetters.S9a, (int) IsolatedLetters.S9a),
                new GlyphMapping((int) GeneralLetters.Dha, (int) IsolatedLetters.Dha),
                new GlyphMapping((int) GeneralLetters.T6a, (int) IsolatedLetters.T6a),
                new GlyphMapping((int) GeneralLetters.T6ha, (int) IsolatedLetters.T6ha),
                new GlyphMapping((int) GeneralLetters.Ain, (int) IsolatedLetters.Ain),
                new GlyphMapping((int) GeneralLetters.Gain, (int) IsolatedLetters.Gain),
                new GlyphMapping((int) GeneralLetters.Fa, (int) IsolatedLetters.Fa),
                new GlyphMapping((int) GeneralLetters.Gaf, (int) IsolatedLetters.Gaf),
                new GlyphMapping((int) GeneralLetters.Kaf, (int) IsolatedLetters.Kaf),
                new GlyphMapping((int) GeneralLetters.Lam, (int) IsolatedLetters.Lam),
                new GlyphMapping((int) GeneralLetters.Meem, (int) IsolatedLetters.Meem),
                new GlyphMapping((int) GeneralLetters.Noon, (int) IsolatedLetters.Noon),
                new GlyphMapping((int) GeneralLetters.Ha, (int) IsolatedLetters.Ha),
                new GlyphMapping((int) GeneralLetters.Waw, (int) IsolatedLetters.Waw),
                new GlyphMapping((int) GeneralLetters.Ya, (int) IsolatedLetters.Ya),
                new GlyphMapping((int) GeneralLetters.AlefMad, (int) IsolatedLetters.AlefMad),
                new GlyphMapping((int) GeneralLetters.TaMarboota, (int) IsolatedLetters.TaMarboota),
                new GlyphMapping((int) GeneralLetters.PersianYa, (int) IsolatedLetters.PersianYa),
                new GlyphMapping((int) GeneralLetters.PersianPe, (int) IsolatedLetters.PersianPe),
                new GlyphMapping((int) GeneralLetters.PersianChe, (int) IsolatedLetters.PersianChe),
                new GlyphMapping((int) GeneralLetters.PersianZe, (int) IsolatedLetters.PersianZe),
                new GlyphMapping((int) GeneralLetters.PersianGaf, (int) IsolatedLetters.PersianGaf),
                new GlyphMapping((int) GeneralLetters.PersianGaf2, (int) IsolatedLetters.PersianGaf2)
            };
        }

        public static int Convert(int toBeConverted)
        {
            foreach (GlyphMapping arabicMap in MapList)
            {
                if (arabicMap.From == toBeConverted)
                {
                    return arabicMap.To;
                }
            }

            return toBeConverted;
        }
    }
}