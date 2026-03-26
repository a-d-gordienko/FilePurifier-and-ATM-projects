using FilePurifier.Core;
using System.Text;

namespace FilePurifier.Tests
{
    public class TextCleanerTests : IDisposable
    {
        private readonly string _testDir;

        public TextCleanerTests()
        {
            // Создаем временную директорию для тестов
            _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDir);
        }

        [Fact]
        public void Process_ShouldRemoveShortWordsAndPunctuation()
        {
            // Arrange
            string inputPath = Path.Combine(_testDir, "simple.txt");
            string expectedPath = Path.Combine(_testDir, "simple_cleaned.txt");
            
            var utf8WithoutBom = new UTF8Encoding(false);
            // "Привет, это тест!" -> "Привет тест!" (если лимит 3 символа, "это" удалится)
            File.WriteAllText(inputPath, "Привет, это тест!", utf8WithoutBom);

            var cleaner = new TextCleaner(removeShortWords: true, minWordLength: 4, removePunctuation: true);

            // Act
            cleaner.Process(inputPath);

            // Assert
            string result = File.ReadAllText(expectedPath);
            // "это" (3 симв) удалено, запятая и воскл. знак удалены.
            Assert.Equal("Привет  тест", result);
        }

        [Fact]
        public void Process_ShouldGlueSplitWordsBetweenBlocks()
        {
            // Arrange
            string inputPath = Path.Combine(_testDir, "split.txt");
            string expectedPath = Path.Combine(_testDir, "split_cleaned.txt");

            // Создаем текст, где слово "LongWord" будет разорвано на границе 4096 байт
            // Заполняем начало пробелами (4094 байта), затем "Lo" | "ngWord"
            StringBuilder sb = new StringBuilder();
            sb.Append(' ', 4094);
            sb.Append("LongWord"); // Итого 4102 символа

            File.WriteAllText(inputPath, sb.ToString(), Encoding.UTF8);

            // Настройка: удалять слова меньше 5 символов. 
            // Если "Lo" и "ngWord" не склеятся, они оба удалятся.
            var cleaner = new TextCleaner(removeShortWords: true, minWordLength: 5, removePunctuation: false);

            // Act
            cleaner.Process(inputPath);

            // Assert
            string result = File.ReadAllText(expectedPath);
            Assert.Contains("LongWord", result);
        }

        [Fact]
        public void Process_ShouldThrowIfFileIsNotText()
        {
            // Arrange
            string binaryPath = Path.Combine(_testDir, "image.png");
            byte[] binaryData = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00]; // PNG header
            File.WriteAllBytes(binaryPath, binaryData);

            var cleaner = new TextCleaner(false, 0, false);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => cleaner.Process(binaryPath));
        }

        [Fact]
        public void Process_KeepPunctuation_WhenFlagIsFalse()
        {
            // Arrange
            string inputPath = Path.Combine(_testDir, "punct.txt");
            string expectedPath = Path.Combine(_testDir, "punct_cleaned.txt");

            var utf8WithoutBom = new UTF8Encoding(false);
            File.WriteAllText(inputPath, "Hello, World!", utf8WithoutBom);

            var cleaner = new TextCleaner(false, 0, removePunctuation: false);

            // Act
            cleaner.Process(inputPath);

            // Assert
            string result = File.ReadAllText(expectedPath);
            Assert.Equal("Hello, World!", result);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }
    }
}

