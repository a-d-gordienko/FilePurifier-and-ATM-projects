using FilePurifier.Core;
using System.IO;
using System.Text;

namespace FilePurifier.Tests;

public class TextFileWriterTests : IDisposable
{
    private readonly string _testDir;

    public TextFileWriterTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void WriteLine_SingleLine_WritesCorrectly()
    {
        // Arrange
        string path = Path.Combine(_testDir, "output.txt");

        using (var writer = new TextFileWriter(path))
        {
            writer.WriteLine("Hello");
        }

        // Assert
        string result = File.ReadAllText(path);
        Assert.Equal("Hello" + Environment.NewLine, result);
    }

    [Fact]
    public void WriteLine_MultipleLines_WritesCorrectly()
    {
        // Arrange
        string path = Path.Combine(_testDir, "output2.txt");

        using (var writer = new TextFileWriter(path))
        {
            writer.WriteLine("Line 1");
            writer.WriteLine("Line 2");
            writer.WriteLine("Line 3");
        }

        // Assert
        string result = File.ReadAllText(path);
        string expected = "Line 1" + Environment.NewLine +
                         "Line 2" + Environment.NewLine +
                         "Line 3" + Environment.NewLine;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void WriteLine_NullLine_WritesEmptyLine()
    {
        // Arrange
        string path = Path.Combine(_testDir, "output3.txt");

        using (var writer = new TextFileWriter(path))
        {
            writer.WriteLine(null);
        }

        // Assert
        string result = File.ReadAllText(path);
        Assert.Equal(Environment.NewLine, result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}