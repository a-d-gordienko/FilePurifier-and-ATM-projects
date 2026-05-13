using FilePurifier.Core;
using Moq;
using System.IO;
using System.Text;

namespace FilePurifier.Tests;

public class TextBlockProcessorTests : IDisposable
{
    private readonly string _testDir;

    public TextBlockProcessorTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void Constructor_CreatesInstance()
    {
        // Act
        var processor = new TextBlockProcessor();

        // Assert
        Assert.NotNull(processor);
    }

    [Fact]
    public void ProcessText_EmptyInput_ProducesEmptyOutput()
    {
        // Arrange
        string path = Path.Combine(_testDir, "empty.txt");
        using var writer = new TextFileWriter(path);

        var processor = new TextBlockProcessor();

        // Act
        var result = processor.ProcessText("", writer, isLastBlock: true);
        writer.Flush();

        // Assert
        Assert.False(result.HasError);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}