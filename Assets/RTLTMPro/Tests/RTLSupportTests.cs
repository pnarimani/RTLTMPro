using NUnit.Framework;
using RTLTMPro;

namespace Tests
{
    public class RTLSupportTests
    {
        [Test]
        public void ArabicTextIsSuccessfulyConverted()
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            const string input = "هَذَا النَّص العربي";
            //const string expected = "ﻲﺑﺮﻌﻟا ﺺﱠﻨﻟا اَﺬَﻫ";
            const string expected = "ﻲﺑﺮﻌﻟﺍ ﺺﱠﻨﻟﺍ ﺍَﺬَﻫ";

            RTLSupport.FixRTL(input, outut, false, false, false);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void FarsiTextIsSuccessfulyConverted()
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            const string input = "متن فارسی";
            //const string expected = "ﯽﺳرﺎﻓ ﻦﺘﻣ";
            const string expected = "ﯽﺳﺭﺎﻓ ﻦﺘﻣ";
            

            RTLSupport.FixRTL(input, outut, true, false, false);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TashkeelIsMaintainedInBeginingOfText()
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            const string input = "صبا";
            // const string expected = "ِﺎﺒﺻ";
            const string expected = "ﺎﺒﺻِ";

            RTLSupport.FixRTL(input, outut, false, false, false);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TashkeelIsMaintainedInMiddleOfText()
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            const string input = "مَرد";
            const string expected = "ﺩﺮَﻣ";

            RTLSupport.FixRTL(input, outut, false, false, false);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void TashkeelIsMaintainedInEndOfText()
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            const string input = "صبحِ";
            const string expected = "ِﺢﺒﺻ";

            RTLSupport.FixRTL(input, outut, false, false, false);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }
    }
}