using FilePurifier.Core;
using System.Text;

namespace FilePurifier.Tests
{
    public class FilePurifierServiceTests : IDisposable
    {
        private readonly string _testDir;

        public FilePurifierServiceTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "ServiceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

        [Fact]
        public async Task ProcessFilesAsync_ShouldProcessMultipleFiles()
        {
            // Arrange
            var files = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string path = Path.Combine(_testDir, $"file_{i}.txt");
                var utf8WithoutBom = new UTF8Encoding(false);
                File.WriteAllText(path, "Это длинное слово и короткое.", utf8WithoutBom);
                files.Add(path);
            }

            // Настройка: удалять слова меньше 5 символов
            var service = new FilePurifierService(true, 5, false);
            var processedFiles = new List<string>();
            var progress = new Progress<string>(path => processedFiles.Add(path));

            // Act
            await service.ProcessFilesAsync(files, progress);

            // Даем небольшую задержку, чтобы Progress<T> успел вызвать колбэк (он асинхронен по своей природе)
            await Task.Delay(100);

            // Assert
            Assert.Equal(files.Count, processedFiles.Count);
            foreach (var file in files)
            {
                string outputPath = Path.Combine(_testDir, Path.GetFileNameWithoutExtension(file) + "_cleaned.txt");
                Assert.True(File.Exists(outputPath), $"Файл {outputPath} не был создан.");

                string content = File.ReadAllText(outputPath);
                Assert.DoesNotContain("Это", content); // "Это" < 5 символов
                Assert.Contains("длинное", content);
            }
        }

        [Fact]
        public async Task ProcessFilesAsync_WithEmptyList_ShouldNotFail()
        {
            // Arrange
            var service = new FilePurifierService(false, 0, false);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() =>
                service.ProcessFilesAsync(Enumerable.Empty<string>()));

            Assert.Null(exception);
        }

        [Fact]
        public async Task ProcessFilesAsync_ReportProgress_ShouldBeThreadSafe()
        {
            // Arrange
            int fileCount = 10;
            var files = new List<string>();
            for (int i = 0; i < fileCount; i++)
            {
                string path = Path.Combine(_testDir, $"batch_{i}.txt");
                var utf8WithoutBom = new UTF8Encoding(false);
                File.WriteAllText(path, "Контент для теста безопасности потоков.", utf8WithoutBom);
                files.Add(path);
            }

            var service = new FilePurifierService(false, 0, false);
            int reportCount = 0;
            var progress = new Progress<string>(_ => Interlocked.Increment(ref reportCount));

            // Act
            await service.ProcessFilesAsync(files, progress);
            await Task.Delay(200); // Ждем асинхронный Progress

            // Assert
            Assert.Equal(fileCount, reportCount);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, true);
        }
    }
}

