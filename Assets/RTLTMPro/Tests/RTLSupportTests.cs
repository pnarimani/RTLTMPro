using NUnit.Framework;
using RTLTMPro;

namespace Tests
{
    public class RTLSupportTests
    {
        [TestCase("هَذَا النَّص العربي", "ﻲﺑﺮﻌﻟﺍ ﺺﱠﻨﻟﺍ ﺍَﺬَﻫ", false, true, true, TestName = "Arabic text is successfully converted")]
        [TestCase("متن فارسی", "ﯽﺳﺭﺎﻓ ﻦﺘﻣ", true, true, true, TestName = "Farsi text is successfully converted")]
        [TestCase("صبا", "ﺎﺒﺻ", false, true, true, TestName = "Tashkeel is maintained in beginning of text")]
        [TestCase("مَرد", "ﺩﺮَﻣ", false, true, true, TestName = "Tashkeel is maintained in middle of text")]
        [TestCase("صبحِ", "ِﺢﺒﺻ", false, true, true, TestName = "Tashkeel is maintained at end of text")]
        [TestCase("العالم", "ﻢﻟﺎﻌﻟﺍ", false, true, true, TestName = "Arabic word conversion")]
        [TestCase("ﺍﻟﻌﺎﻟﻢ", "ﻢﻟﺎﻌﻟﺍ", false, true, true, TestName = "Arabic presentation form word should not convert")]
        [TestCase("اعلام لام", "ﻡﻻ ﻡﻼﻋﺍ", false, true, true, TestName = "Special lam handling base case")]
        [TestCase("اعلآم لآم", "ﻡﻵ ﻡﻶﻋﺍ", false, true, true, TestName = "Special lam handling AlefMaddaAbove case")]
        [TestCase("اعلأم لأم", "ﻡﻷ ﻡﻸﻋﺍ", false, true, true, TestName = "Special lam handling AlefHamzaAbove case")]
        [TestCase("اعلإم لإم", "ﻡﻹ ﻡﻺﻋﺍ", false, true, true, TestName = "Special lam handling AlefHamzaBelow case")]
        public void FixRTL(string input, string expected, bool farsi, bool fixTags, bool preserveNumbers)
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            
            RTLSupport.FixRTL(input, outut, farsi, fixTags, preserveNumbers);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }
    }
}