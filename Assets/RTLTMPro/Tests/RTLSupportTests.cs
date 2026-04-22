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
        [TestCase("ﻣﺘﻦ ﻓﺎﺭﺳﯽ", "ﯽﺳﺭﺎﻓ ﻦﺘﻣ", true, true, true, TestName = "Persian presentation form word should not convert")]
        public void FixRTL(string input, string expected, bool farsi, bool fixTags, bool preserveNumbers)
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            
            RTLSupport.FixRTL(input, outut, farsi, fixTags, preserveNumbers);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }
    }
}