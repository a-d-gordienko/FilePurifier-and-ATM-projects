using FilePurifier.Core;

namespace FilePurifier.Tests;

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
            System.IO.File.WriteAllText(path, "Это длинное слово и короткое.");
            files.Add(path);
        }

        var service = new FilePurifierService();
        var options = new TextCleanerOptions
        {
            RemoveShortWords = true,
            MinWordLength = 5,
            RemovePunctuation = false
        };

        var processedFiles = new List<string>();
        var progress = new Progress<FileProcessingStatus>(status =>
        {
            if (status.Result == FileProcessingResult.Success)
                processedFiles.Add(status.FilePath);
        });

        // Act
        await service.ProcessFilesAsync(files, options, progress);

        // Assert
        Assert.Equal(files.Count, processedFiles.Count);
    }

    [Fact]
    public async Task ProcessFilesAsync_WithEmptyList_ShouldNotFail()
    {
        // Arrange
        var service = new FilePurifierService();
        var options = new TextCleanerOptions
        {
            RemoveShortWords = false,
            MinWordLength = 0,
            RemovePunctuation = false
        };

        // Act & Assert
        var exception = await Record.ExceptionAsync(() =>
            service.ProcessFilesAsync(Enumerable.Empty<string>(), options));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ProcessFilesAsync_ReportProgress_ShouldWork()
    {
        // Arrange
        int fileCount = 10;
        var files = new List<string>();
        for (int i = 0; i < fileCount; i++)
        {
            string path = Path.Combine(_testDir, $"batch_{i}.txt");
            System.IO.File.WriteAllText(path, "Контент для теста.");
            files.Add(path);
        }

        var service = new FilePurifierService();
        var options = new TextCleanerOptions
        {
            RemoveShortWords = false,
            MinWordLength = 0,
            RemovePunctuation = false
        };

        int reportCount = 0;
        var progress = new Progress<FileProcessingStatus>(_ => Interlocked.Increment(ref reportCount));

        // Act
        await service.ProcessFilesAsync(files, options, progress);

        // Assert
        Assert.Equal(fileCount, reportCount);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}