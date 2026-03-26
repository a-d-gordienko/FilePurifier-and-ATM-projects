using FilePurifier.Core;
using System.Text;

namespace FilePurifier.Tests
{
    public class FileLoaderTests : IDisposable
    {
        private readonly string _testFile;

        public FileLoaderTests()
        {
            _testFile = Path.GetTempFileName();
        }

        [Fact]
        public void NextBlock_ShouldReadContentCorrectly()
        {
            // Arrange
            string content = "Hello, .NET 8!";
            File.WriteAllText(_testFile, content);
            using var loader = new FileLoader(_testFile);

            // Act
            var result = loader.NextBlock();
            var span = loader.GetBufferSpan();
            string resultText = Encoding.UTF8.GetString(span);

            // Assert
            Assert.Equal(NextBlockError.EndBlock, result);
            Assert.Equal(content, resultText);
        }

        [Fact]
        public void NextBlock_ShouldHandleMultiBlockFiles()
        {
            // Arrange: создаем файл размером 1.5 буфера
            byte[] data = new byte[(int)(FileLoader.BufferSize * 1.5)];
            new Random().NextBytes(data);
            File.WriteAllBytes(_testFile, data);

            using var loader = new FileLoader(_testFile);

            // Act & Assert
            // Первый блок
            Assert.Equal(NextBlockError.Ok, loader.NextBlock());
            Assert.Equal(FileLoader.BufferSize, loader.GetBufferSpan().Length);

            // Второй (последний) блок
            Assert.Equal(NextBlockError.EndBlock, loader.NextBlock());
            Assert.Equal(FileLoader.BufferSize * 0.5, loader.GetBufferSpan().Length);
        }

        [Fact]
        public void GetLastException_ShouldReturnException_WhenFileNotFound()
        {
            // Arrange
            string fakePath = "non_existent_file.txt";

            // Act
            using var loader = new FileLoader(fakePath);

            // Assert
            Assert.NotNull(loader.GetLastException());
            Assert.Equal(NextBlockError.Error, loader.NextBlock());
        }

        [Fact]
        public void GetBufferSpan_ShouldBeEmpty_OnInitialState()
        {
            // Arrange
            File.WriteAllText(_testFile, "some text");
            using var loader = new FileLoader(_testFile);

            // Act
            var span = loader.GetBufferSpan();

            // Assert
            Assert.True(span.IsEmpty);
        }

        public void Dispose()
        {
            if (File.Exists(_testFile)) File.Delete(_testFile);
        }
    }

    public class Utf8EncodingTests : IDisposable
    {
        private readonly string _tempFile;

        public Utf8EncodingTests()
        {
            _tempFile = Path.GetTempFileName();
        }

        [Fact]
        public void NextBlock_ShouldRollback_WhenRussianLetterIsSplit()
        {
            // Arrange
            // Буква 'Я' в UTF-8 это 2 байта: 0xD0 0xAF
            // Мы создадим файл, где 0xD0 будет на позиции 4095 (конец буфера), 
            // а 0xAF на позиции 4096 (начало следующего буфера).

            byte[] padding = new byte[4095]; // Заполняем нулями до конца буфера - 1
            byte[] russianYa = [0xD0, 0xAF];

            using (var fs = File.OpenWrite(_tempFile))
            {
                fs.Write(padding);
                fs.Write(russianYa);
            }

            using var loader = new FileLoader(_tempFile);

            // Act
            // Читаем первый блок (4096 байт)
            var result = loader.NextBlock();
            var span = loader.GetBufferSpan();

            // Assert
            // 1. Метод не должен считать этот блок последним, так как мы "откатились"
            Assert.Equal(NextBlockError.Ok, result);

            // 2. Длина должна быть 4095 (символ 0xD0 должен быть отброшен)
            Assert.Equal(4095, span.Length);

            // 3. Проверяем, что в следующем блоке мы прочитаем букву целиком
            loader.NextBlock();
            var nextSpan = loader.GetBufferSpan();
            string decoded = Encoding.UTF8.GetString(nextSpan);

            Assert.Equal("Я", decoded);
        }

        [Fact]
        public void NextBlock_ShouldHandleEmojiSplit_FourBytes()
        {
            // Arrange
            // Эмодзи 🧩 (Puzzle Piece) в UTF-8: 0xF0 0x9F 0xA7 0xA9
            // Нам нужно создать разрыв. Оставим первые 3 байта в первом блоке (4093, 4094, 4095),
            // а последний байт 0xA9 уйдет в следующий блок.

            byte[] padding = new byte[4093]; // 4093 байта отступа
            byte[] emoji = [0xF0, 0x9F, 0xA7, 0xA9]; // 4 байта

            // Итого файл: 4093 (пусто) + 4 (эмодзи) = 4097 байт.
            // Первый вызов Read(4096) прочитает 4093 байта отступа и 3 байта эмодзи.
            File.WriteAllBytes(_tempFile, [.. padding, .. emoji]);

            using var loader = new FileLoader(_tempFile);

            // Act
            // 1. Читаем первый блок
            loader.NextBlock();
            int firstReadLength = loader.GetBufferSpan().Length;

            // Assert
            // Лоадер должен увидеть в конце 0xF0 0x9F 0xA7 (начало 4-байтового символа)
            // Понять, что 4-го байта нет, и откатиться на 3 байта назад.
            // Ожидаемая длина: 4096 - 3 = 4093
            Assert.Equal(4093, firstReadLength);

            // 2. Читаем второй блок
            loader.NextBlock();
            // Теперь лоадер должен прочитать все 4 байта эмодзи с новой позиции
            string decoded = Encoding.UTF8.GetString(loader.GetBufferSpan());

            Assert.Equal("🧩", decoded);
        }


        public void Dispose()
        {
            if (File.Exists(_tempFile)) File.Delete(_tempFile);
        }
    }
}
