namespace FilePurifier.Core;

public enum ReadBlockResult
{
    Ok,
    EndOfFile,
    Error
}

public interface IFileLoader : IDisposable
{
    ReadBlockResult ReadBlock();
    ReadOnlySpan<byte> GetBufferSpan();
    bool HasError { get; }
    Exception? GetLastException();
}

public interface IFileWriter : IDisposable
{
    void WriteLine(string? line);
    void Flush();
}

public interface ITextProcessor
{
    TextProcessingResult ProcessBlock(
        ReadOnlySpan<byte> buffer,
        IFileWriter writer,
        bool isLastBlock);
}

public readonly struct TextProcessingResult
{
    public string LeftoverWord { get; init; }
    public bool HasError { get; init; }
    public Exception? Error { get; init; }

    public static TextProcessingResult Success(string leftoverWord = "") =>
        new() { LeftoverWord = leftoverWord };

    public static TextProcessingResult ErrorResult(Exception ex) =>
        new() { HasError = true, Error = ex };
}

public interface ITextCleaner
{
    TextCleanerOptions Options { get; }
    TextProcessingResult ProcessBlock(
        ReadOnlySpan<byte> buffer,
        IFileWriter writer,
        bool isLastBlock,
        string leftoverWord);
}

public readonly struct TextCleanerOptions
{
    public required bool RemoveShortWords { get; init; }
    public required int MinWordLength { get; init; }
    public required bool RemovePunctuation { get; init; }
}