using FilePurifier.Core;
using System.IO;

namespace FilePurifier.Tests;

public class FileLoaderTests : IDisposable
{
    private readonly string _testDir;

    public FileLoaderTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void Constructor_ValidFile_OpensSuccessfully()
    {
        // Arrange
        string path = Path.Combine(_testDir, "test.txt");
        System.IO.File.WriteAllText(path, "Hello World");

        // Act
        using var loader = new FileLoader(path);

        // Assert
        Assert.False(loader.HasError);
        Assert.Null(loader.GetLastException());
    }

    [Fact]
    public void ReadBlock_ValidFile_ReturnsOkOrEndOfFile()
    {
        // Arrange
        string path = Path.Combine(_testDir, "test.txt");
        System.IO.File.WriteAllText(path, "Hello World");

        using var loader = new FileLoader(path);

        // Act
        var result = loader.ReadBlock();

        // Assert - file fits in one block, so it could be Ok or EndOfFile depending on file size vs buffer
        Assert.True(result == ReadBlockResult.Ok || result == ReadBlockResult.EndOfFile);
    }

    [Fact]
    public void ReadBlock_EmptyFile_ReturnsEndOfFile()
    {
        // Arrange
        string path = Path.Combine(_testDir, "empty.txt");
        System.IO.File.WriteAllText(path, "");

        using var loader = new FileLoader(path);

        // Act
        var result = loader.ReadBlock();

        // Assert
        Assert.Equal(ReadBlockResult.EndOfFile, result);
    }

    [Fact]
    public void ReadBlock_MissingFile_SetsError()
    {
        // Arrange
        string path = Path.Combine(_testDir, "nonexistent.txt");

        // Act
        using var loader = new FileLoader(path);

        // Assert
        Assert.True(loader.HasError);
        Assert.NotNull(loader.GetLastException());
    }

    [Fact]
    public void ReadBlock_Utf8MultibyteChars_HandlesCorrectly()
    {
        // Arrange
        string path = Path.Combine(_testDir, "utf8.txt");
        System.IO.File.WriteAllText(path, "Привет мир");

        using var loader = new FileLoader(path);

        // Act
        var result = loader.ReadBlock();
        var buffer = loader.GetBufferSpan();

        // Assert
        Assert.True(result == ReadBlockResult.Ok || result == ReadBlockResult.EndOfFile);
        Assert.False(buffer.IsEmpty);
    }

    [Fact]
    public void GetBufferSpan_AfterRead_ReturnsNonEmpty()
    {
        // Arrange
        string path = Path.Combine(_testDir, "buffer.txt");
        System.IO.File.WriteAllText(path, "Test content");

        using var loader = new FileLoader(path);
        loader.ReadBlock();

        // Act
        var span = loader.GetBufferSpan();

        // Assert
        Assert.False(span.IsEmpty);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}