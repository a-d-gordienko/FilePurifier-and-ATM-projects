using FilePurifier.Core;
using System.Text;

namespace FilePurifier.Tests
{
    public class TextBlockInspectorTests
    {
        [Fact]
        public void IsText_NormalText_ReturnsTrue()
        {
            // Arrange
            byte[] data = Encoding.UTF8.GetBytes("Hello, this is a normal string with numbers 123.");

            // Act
            bool result = TextBlockInspector.IsText(data);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsText_WithUtf8Bom_ReturnsTrue()
        {
            // Arrange: BOM (EF BB BF) + "Text"
            byte[] data = [0xEF, 0xBB, 0xBF, 0x54, 0x65, 0x78, 0x74];

            // Act
            bool result = TextBlockInspector.IsText(data);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsText_ContainsNullByte_ReturnsFalse()
        {
            // Arrange: "Hi" + Null-byte + "End"
            byte[] data = [0x48, 0x69, 0x00, 0x45, 0x6E, 0x64];

            // Act
            bool result = TextBlockInspector.IsText(data);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsText_BinaryControlCharacters_ReturnsFalse()
        {
            // Arrange: байт 0x01 (Start of Heading) — не является текстом
            byte[] data = [0x48, 0x65, 0x01, 0x6C, 0x6F];

            // Act
            bool result = TextBlockInspector.IsText(data);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(9)]  // Tab
        [InlineData(10)] // Line Feed (\n)
        [InlineData(13)] // Carriage Return (\r)
        public void IsText_AllowedControlChars_ReturnsTrue(byte controlChar)
        {
            // Arrange
            byte[] data = [0x41, controlChar, 0x42]; // "A" + control + "B"

            // Act
            bool result = TextBlockInspector.IsText(data);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsText_EmptyBuffer_ReturnsFalse()
        {
            // Act
            bool result = TextBlockInspector.IsText(ReadOnlySpan<byte>.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsText_VeryShortBuffer_ReturnsFalseIfBinary()
        {
            // Arrange: всего 1 байт, и он "бинарный" (0x02)
            byte[] data = [0x02];

            // Act
            bool result = TextBlockInspector.IsText(data);

            // Assert
            Assert.False(result);
        }
    }
}
