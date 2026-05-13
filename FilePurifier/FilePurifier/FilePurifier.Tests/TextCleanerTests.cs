using FilePurifier.Core;
using System.IO;
using System.Text;

namespace FilePurifier.Tests;

public class TextCleanerTests : IDisposable
{
    private readonly string _testDir;

    public TextCleanerTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void ProcessFile_ValidTextFile_ProducesOutput()
    {
        // Arrange
        string inputPath = Path.Combine(_testDir, "simple.txt");
        string expectedPath = Path.Combine(_testDir, "simple_cleaned.txt");

        System.IO.File.WriteAllText(inputPath, "Привет, это тест!", new UTF8Encoding(false));

        var service = new TextCleanerService(new TextCleanerOptions
        {
            RemoveShortWords = true,
            MinWordLength = 4,
            RemovePunctuation = true
        });

        // Act
        service.ProcessFile(inputPath);

        // Assert
        Assert.True(System.IO.File.Exists(expectedPath));
    }

    [Fact]
    public void ProcessFile_Utf8MultibyteChars_HandlesCorrectly()
    {
        // Arrange
        string inputPath = Path.Combine(_testDir, "utf8.txt");
        System.IO.File.WriteAllText(inputPath, "Привет мир", Encoding.UTF8);

        var service = new TextCleanerService(new TextCleanerOptions
        {
            RemoveShortWords = false,
            MinWordLength = 0,
            RemovePunctuation = false
        });

        // Act
        service.ProcessFile(inputPath);

        // Assert
        string outputPath = Path.Combine(_testDir, "utf8_cleaned.txt");
        Assert.True(System.IO.File.Exists(outputPath));
    }

    [Fact]
    public void ProcessFile_BinaryFile_ThrowsException()
    {
        // Arrange
        string binaryPath = Path.Combine(_testDir, "image.png");
        byte[] binaryData = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
        System.IO.File.WriteAllBytes(binaryPath, binaryData);

        var service = new TextCleanerService(new TextCleanerOptions
        {
            RemoveShortWords = false,
            MinWordLength = 0,
            RemovePunctuation = false
        });

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.ProcessFile(binaryPath));
    }

    [Fact]
    public void ProcessFile_SmallFile_ProducesOutput()
    {
        // Arrange
        string inputPath = Path.Combine(_testDir, "small.txt");
        System.IO.File.WriteAllText(inputPath, "Hello world");

        var service = new TextCleanerService(new TextCleanerOptions
        {
            RemoveShortWords = false,
            MinWordLength = 0,
            RemovePunctuation = false
        });

        // Act
        service.ProcessFile(inputPath);

        // Assert
        string outputPath = Path.Combine(_testDir, "small_cleaned.txt");
        Assert.True(System.IO.File.Exists(outputPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}