using System.Collections.Generic;
// ReSharper disable IdentifierTypo

namespace RTLTMPro
{
    public static class TashkeelFixer
    {
        private static readonly List<TashkeelLocation> TashkeelLocations = new List<TashkeelLocation>(100);

        private static readonly string ShaddaDammatan = new string(
            new[] {(char)TashkeelCharacters.Shadda, (char)TashkeelCharacters.Dammatan});

        private static readonly string ShaddaKasratan = new string(
            new[] {(char)TashkeelCharacters.Shadda, (char)TashkeelCharacters.Kasratan});

        private static readonly string ShaddaSuperscriptAlef = new string(
            new[] {(char)TashkeelCharacters.Shadda, (char)TashkeelCharacters.SuperscriptAlef});

        private static readonly string ShaddaFatha = new string(
            new[] {(char)TashkeelCharacters.Shadda, (char)TashkeelCharacters.Fatha});

        private static readonly string ShaddaDamma = new string(
            new[] {(char)TashkeelCharacters.Shadda, (char)TashkeelCharacters.Damma});

        private static readonly string ShaddaKasra = new string(
            new[] {(char)TashkeelCharacters.Shadda, (char)TashkeelCharacters.Kasra});

        private static readonly string ShaddaWithFathaIsolatedForm =
            ((char)TashkeelCharacters.ShaddaWithFathaIsolatedForm).ToString();

        private static readonly string ShaddaWithDammaIsolatedForm =
            ((char)TashkeelCharacters.ShaddaWithDammaIsolatedForm).ToString();

        private static readonly string ShaddaWithKasraIsolatedForm =
            ((char)TashkeelCharacters.ShaddaWithKasraIsolatedForm).ToString();

        private static readonly string ShaddaWithDammatanIsolatedForm =
            ((char)TashkeelCharacters.ShaddaWithDammatanIsolatedForm).ToString();

        private static readonly string ShaddaWithKasratanIsolatedForm =
            ((char)TashkeelCharacters.ShaddaWithKasratanIsolatedForm).ToString();

        private static readonly string ShaddaWithSuperscriptAlefIsolatedForm =
            ((char)TashkeelCharacters.ShaddaWithSuperscriptAlefIsolatedForm).ToString();

        /// <summary>
        ///     Removes tashkeel from text.
        /// </summary>
        public static void RemoveTashkeel(FastStringBuilder input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                switch ((TashkeelCharacters)input.Get(i))
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


            input.RemoveAll((char)TashkeelCharacters.Fathan);
            input.RemoveAll((char)TashkeelCharacters.Dammatan);
            input.RemoveAll((char)TashkeelCharacters.Kasratan);
            input.RemoveAll((char)TashkeelCharacters.Fatha);
            input.RemoveAll((char)TashkeelCharacters.Damma);
            input.RemoveAll((char)TashkeelCharacters.Kasra);
            input.RemoveAll((char)TashkeelCharacters.Shadda);
            input.RemoveAll((char)TashkeelCharacters.Sukun);
            input.RemoveAll((char)TashkeelCharacters.MaddahAbove);
            input.RemoveAll((char)TashkeelCharacters.ShaddaWithFathaIsolatedForm);
            input.RemoveAll((char)TashkeelCharacters.ShaddaWithDammaIsolatedForm);
            input.RemoveAll((char)TashkeelCharacters.ShaddaWithKasraIsolatedForm);
            input.RemoveAll((char)TashkeelCharacters.SuperscriptAlef);
        }

        /// <summary>
        ///     Restores removed tashkeel.
        /// </summary>
        public static void RestoreTashkeel(FastStringBuilder letters)
        {
            int letterWithTashkeelTracker = 0;
            foreach (TashkeelLocation location in TashkeelLocations)
            {
                letters.Insert(location.Position + letterWithTashkeelTracker, location.Tashkeel);
                //letterWithTashkeelTracker++;
            }

            /*
             * Fix of https://github.com/mnarimani/RTLTMPro/issues/13
             * The workaround is to replace Shadda + Another Tashkeel with combined form 
             */
            letters.Replace(ShaddaFatha, ShaddaWithFathaIsolatedForm);

            letters.Replace(ShaddaDamma, ShaddaWithDammaIsolatedForm);

            letters.Replace(ShaddaKasra, ShaddaWithKasraIsolatedForm);

            letters.Replace(ShaddaDammatan, ShaddaWithDammatanIsolatedForm);

            letters.Replace(ShaddaKasratan, ShaddaWithKasratanIsolatedForm);

            letters.Replace(ShaddaSuperscriptAlef, ShaddaWithSuperscriptAlefIsolatedForm);

            TashkeelLocations.Clear();
        }
    }
}