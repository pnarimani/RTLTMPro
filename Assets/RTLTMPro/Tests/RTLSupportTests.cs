using NUnit.Framework;
using RTLTMPro;

namespace Tests
{
    public class RTLSupportTests
    {   // Base Tests
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
        // Rich Text Scene
        [TestCase("متن <color=yellow>زرد</color> ساده", "ﻩﺩﺎﺳ >roloc/<ﺩﺭﺯ>wolley=roloc< ﻦﺘﻣ", true, true, true, TestName = "Rich text scene: yellow text")]
        [TestCase("کلمه‌ها ميتوانند در وسط جمله <size=100>بزرگ</size> باشند", "ﺪﻨﺷﺎﺑ >ezis/<ﮒﺭﺰﺑ>001=ezis< ﻪﻠﻤﺟ ﻂﺳﻭ ﺭﺩ ﺪﻨﻧﺍﻮﺘﯿﻣ ﺎﻫﻪﻤﻠﮐ", true, true, true, TestName = "Rich text scene: large text")]
        [TestCase("کلمه‌ها حتي ميتوانند <voffset=1em>بالا</voffset> يا <voffset=-1em>پايين</voffset> باشند", "ﺪﻨﺷﺎﺑ >tesffov/<ﻦﯿﯾﺎﭘ>me1-=tesffov< ﺎﯾ >tesffov/<ﻻﺎﺑ>me1=tesffov< ﺪﻨﻧﺍﻮﺘﯿﻣ ﯽﺘﺣ ﺎﻫﻪﻤﻠﮐ", true, true, true, TestName = "Rich text scene: sub and super text")]
        [TestCase("ﺗﺴﺖ <color=#FAF>ﺭﻧﮕﯽ<b><i>ﺑﺰﺭﮒ</b></color>ﮐﺞ</i>  <sprite index=1/>", ">/1=xedni etirps<  >i/<ﺞﮐ>roloc/<>b/<ﮒﺭﺰﺑ>i<>b<ﯽﮕﻧﺭ>FAF#=roloc< ﺖﺴﺗ", true, true, true, TestName = "Rich text scene: color, emoji, bold and italic text")]
        // Demo Persian Scene
        [TestCase("متن ساده، ویرگول، دو نقطه: اعلام آزادی مردم", "ﻡﺩﺮﻣ ﯼﺩﺍﺯﺁ ﻡﻼﻋﺍ :ﻪﻄﻘﻧ ﻭﺩ ،ﻝﻮﮔﺮﯾﻭ ،ﻩﺩﺎﺳ ﻦﺘﻣ", true, true, true, TestName = "Demo Persian scene: comma and colon")]
        [TestCase("متن فارسی and english در کنار هم", "ﻢﻫ ﺭﺎﻨﮐ ﺭﺩ and english ﯽﺳﺭﺎﻓ ﻦﺘﻣ", true, true, true, TestName = "Demo Persian scene: mixed with english")]
        [TestCase("عدد انگلیسی 123 و 123.456 و 123,456 و (123) و (123.456) و (123,456) پایان.", 
                  ".ﻥﺎﯾﺎﭘ (123,456) ﻭ (123.456) ﻭ (123) ﻭ 123,456 ﻭ 123.456 ﻭ 123 ﯽﺴﯿﻠﮕﻧﺍ ﺩﺪﻋ", true, true, true, TestName = "Demo Persian scene: english numbers")]
        // [TestCase("عدد فارسی 123 و 123.456 و 123,456 و (123) و (123.456) و (123,456) و [123] و [123.456] و [123,456] پایان.", ".ﻥﺎﯾﺎﭘ [۴۵۶,۱۲۳] ﻭ [۴۵۶.۱۲۳] ﻭ [۱۲۳] ﻭ (۴۵۶,۱۲۳) ﻭ (۴۵۶.۱۲۳) ﻭ (۱۲۳) ﻭ ۴۵۶,۱۲۳ ﻭ ۴۵۶.۱۲۳ ﻭ ۱۲۳ ﯽﺳﺭﺎﻓ ﺩﺪﻋ", true, true, false, TestName = "Demo Persian scene: farsi numbers")]
        [TestCase("متن با (پرانتز) {گیومه} و [براکت] و «کوت»", "«ﺕﻮﮐ» ﻭ [ﺖﮐﺍﺮﺑ] ﻭ {ﻪﻣﻮﯿﮔ} (ﺰﺘﻧﺍﺮﭘ) ﺎﺑ ﻦﺘﻣ", true, true, true, TestName = "Demo Persian scene: parentheses, brackets")]
        [TestCase("نيم‌فاصله: مي‌خواهم - نيازمندي‌ها فاصله: مي خواهم - نيازمندي ها", "ﺎﻫ ﯼﺪﻨﻣﺯﺎﯿﻧ - ﻢﻫﺍﻮﺧ ﯽﻣ :ﻪﻠﺻﺎﻓ ﺎﻫﯼﺪﻨﻣﺯﺎﯿﻧ - ﻢﻫﺍﻮﺧﯽﻣ :ﻪﻠﺻﺎﻓﻢﯿﻧ", true, true, true, TestName = "Demo Persian scene: zero width non joiner")]
        // Demo Arabic Scene
        [TestCase("تشکیل: ببَبِبُبًبٍبٌبْبّبَّبِّبُّبًّبٍّبٌّب، منظَّم، المؤثرات الصوتية", "ﺔﻴﺗﻮﺼﻟﺍ ﺕﺍﺮﺛﺆﻤﻟﺍ ،ﻢﱠﻈﻨﻣ ،ﺐﱞﺒﱟﺒًّﺒﱡﺒﱢﺒﱠﺒّﺒْﺒٌﺒٍﺒًﺒُﺒِﺒَﺒﺑ :ﻞﻴﮑﺸﺗ", false, true, true, TestName = "Demo Arabic scene: tashkeel")]
        [TestCase("الكلمات العربية and English جنباً إلى جنب", "ﺐﻨﺟ ﻰﻟﺇ ًﺎﺒﻨﺟ and English ﺔﻴﺑﺮﻌﻟﺍ ﺕﺎﻤﻠﻜﻟﺍ", false, true, true, TestName = "Demo Arabic scene: mixed with english")]
        // Demo Hebrew Scene
        [TestCase("חבילה זו תומכת כעת בשפה העברית!", "!תירבעה הפשב תעכ תכמות וז הליבח", false, true, true, TestName = "Demo Hebrew scene: base")]
        [TestCase("לפניכם טקסט מעורבב של English וטקסט עברי.", ".ירבע טסקטו English לש בברועמ טסקט םכינפל", false, true, true, TestName = "Demo Hebrew scene: mixed with english")]
        public void FixRTL(string input, string expected, bool farsi, bool fixTags, bool preserveNumbers)
        {
            FastStringBuilder outut = new FastStringBuilder(RTLSupport.DefaultBufferSize);
            
            RTLSupport.FixRTL(input, outut, farsi, fixTags, preserveNumbers);
            string result = outut.ToString();

            Assert.AreEqual(expected, result);
        }
    }
}