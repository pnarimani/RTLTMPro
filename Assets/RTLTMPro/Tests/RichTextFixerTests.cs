using System.Text;
using NUnit.Framework;

namespace RTLTMPro.Tests
{
    public class RichTextFixerTests
    {
        [Test]
        public void FindTag_FindsSimpleOpeningTag()
        {
            // Arrange
            var text = new FastStringBuilder("text <opening> text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out int type, out _);
            
            // Assert
            Assert.AreEqual(1, type);
            Assert.AreEqual(5, start);
            Assert.AreEqual(13, end);
        }
        
        [Test]
        public void FindTag_DoesntFindsTagWithSpaceInside()
        {
            // Arrange
            var text = new FastStringBuilder("text <ope ning> text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out int type, out _);
            
            // Assert
            Assert.AreEqual(0, type);
            Assert.AreEqual(0, start);
            Assert.AreEqual(0, end);
        }
        
        [Test]
        public void FindTag_FindsOpeningTagWithValue()
        {
            // Arrange
            var text = new FastStringBuilder("text <opening=12m> text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out var type, out _);
            
            // Assert
            Assert.AreEqual(1, type);
            Assert.AreEqual(5, start);
            Assert.AreEqual(17, end);
        }
        
        [Test]
        public void FindTag_ProducesTheSameHash_ForOpeningTagsWithDifferentValues()
        {
            // Arrange
            var text1 = new FastStringBuilder("text <opening=12m> text");
            var text2 = new FastStringBuilder("text <opening=18s> text");
            
            // Act
            RichTextFixer.FindTag(text1, 0, out _, out _, out _, out var hashCode1);
            RichTextFixer.FindTag(text2, 0, out _, out _, out _, out var hashCode2);
            
            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [Test]
        public void FindTag_FindsSimpleSelfContainedTag()
        {
            // Arrange
            var text = new FastStringBuilder("text <opening/> text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out int type, out _);
            
            // Assert
            Assert.AreEqual(3, type);
            Assert.AreEqual(5, start);
            Assert.AreEqual(14, end);
        }
        
        [Test]
        public void FindTag_FindsSelfContainedTagWithValue()
        {
            // Arrange
            var text = new FastStringBuilder("text <opening=15m/> text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out int type, out _);
            
            // Assert
            Assert.AreEqual(3, type);
            Assert.AreEqual(5, start);
            Assert.AreEqual(18, end);
        }

        [Test]
        public void FindTag_ProducesTheSameHash_ForSelfContainedTagsWithDifferentValues()
        {
            // Arrange
            var text1 = new FastStringBuilder("text <opening=15m/> text");
            var text2 = new FastStringBuilder("text <opening=20d/> text");
            
            // Act
            RichTextFixer.FindTag(text1, 0, out _, out _, out _, out int hash1);
            RichTextFixer.FindTag(text2, 0, out _, out _, out _, out int hash2);
            
            // Assert
            Assert.AreEqual(hash1 ,hash2);
        }
        
        [Test]
        public void FindTag_FindsClosingTag()
        {
            // Arrange
            var text = new FastStringBuilder("text </closing> text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out int type, out _);
            
            // Assert
            Assert.AreEqual(2, type);
            Assert.AreEqual(5, start);
            Assert.AreEqual(14, end);
        }

        [Test]
        public void FindTag_ProducesTheSameHash_ForSameOpeningAndClosingTag()
        {
            // Arrange
            var text1 = new FastStringBuilder("text <tag=15m/> text");
            var text2 = new FastStringBuilder("text </tag> text");
            
            // Act
            RichTextFixer.FindTag(text1, 0, out _, out _, out _, out int hash1);
            RichTextFixer.FindTag(text2, 0, out _, out _, out _, out int hash2);
            
            // Assert
            Assert.AreEqual(hash1 ,hash2);   
        }

        [Test]
        public void FindTag_ReturnsZero_WhenNoTagIsFound()
        {
            // Arrange
            var text = new FastStringBuilder("text");
            
            // Act
            RichTextFixer.FindTag(text, 0, out var start, out var end, out int type, out int hashCode);
            
            // Assert
            Assert.AreEqual(0, start);
            Assert.AreEqual(0, end);
            Assert.AreEqual(0, type);
            Assert.AreEqual(0, hashCode);
        }

        [Test]
        public void FindTag_StartFromTheProvidedStartPosition()
        {
            // Arrange
            var text = new FastStringBuilder(" <tag> text");
            
            // Act
            RichTextFixer.FindTag(text, 6, out var start, out var end, out int type, out int hashCode);
            
            // Assert
            Assert.AreEqual(0, start);
            Assert.AreEqual(0, end);
            Assert.AreEqual(0, type);
            Assert.AreEqual(0, hashCode);
        }

        [Test]
        public void Fix_Reverses_SelfContainedTags()
        {
            // Arrange
            var text = new FastStringBuilder("text <self-contained/> text");
            
            // Act
            RichTextFixer.Fix(text);
            
            // Assert
            Assert.AreEqual("text >/deniatnoc-fles< text", text.ToString());
        }

        [Test]
        public void Fix_Reverses_OpenTag_ThatDoesntHaveClosingTag()
        {
            // Arrange
            var text = new FastStringBuilder("text <open> text");
            
            // Act
            RichTextFixer.Fix(text);
            
            // Assert
            Assert.AreEqual("text >nepo< text", text.ToString());
        }
        
        [Test]
        public void Fix_Reverses_SimpleOpenAndClosingTag()
        {
            // Arrange
            var text = new FastStringBuilder("text </open> text <open>");
            
            // Act
            RichTextFixer.Fix(text);
            
            // Assert
            Assert.AreEqual("text >nepo/< text >nepo<", text.ToString());
        }
        
        [Test]
        public void Fix_Reverses_SimpleOpenAndClosingTagWithValue()
        {
            // Arrange
            var text = new FastStringBuilder("text </open> text <open=134>");
            
            // Act
            RichTextFixer.Fix(text);
            
            // Assert
            Assert.AreEqual("text >nepo/< text >431=nepo<", text.ToString());
        }
    }
}
