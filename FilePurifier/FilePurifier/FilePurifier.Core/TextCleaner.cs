using System.Buffers;
using System.IO;
using System.Text;

namespace FilePurifier.Core;

public class TextCleanerOrchestrator : ITextProcessor
{
    private readonly TextCleanerOptions _options;
    private string _leftoverWord = string.Empty;

    public TextCleanerOrchestrator(TextCleanerOptions options)
    {
        _options = options;
    }

    public TextCleanerOptions Options => _options;

    public TextProcessingResult ProcessBlock(
        ReadOnlySpan<byte> buffer,
        IFileWriter writer,
        bool isLastBlock)
    {
        string text = Encoding.UTF8.GetString(buffer);
        var span = text.AsSpan();

        if (!string.IsNullOrEmpty(_leftoverWord))
        {
            span = string.Concat(_leftoverWord, text).AsSpan();
            _leftoverWord = string.Empty;
        }

        var output = new StringBuilder(span.Length);
        var result = ProcessText(span, isLastBlock);

        if (result.HasError)
            return result;

        _leftoverWord = result.LeftoverWord;
        return result;
    }

    private TextProcessingResult ProcessText(ReadOnlySpan<char> span, bool isLastBlock)
    {
        return TextProcessingResult.Success();
    }
}

public class TextCleanerService
{
    private readonly TextCleanerOptions _options;

    public TextCleanerService(TextCleanerOptions options)
    {
        _options = options;
    }

    public void ProcessFile(string inputPath)
    {
        byte[] fileBytes = File.ReadAllBytes(inputPath);
        if (!TextBlockInspector.IsText(fileBytes.AsSpan()) && fileBytes.Length > 0)
        {
            throw new InvalidOperationException($"Файл {inputPath} не является текстовым.");
        }

        string outputPath = GenerateOutputPath(inputPath);
        string leftoverWord = string.Empty;

        using var loader = new FileLoader(inputPath);
        using var writer = new TextFileWriter(outputPath);

        var processor = new TextCleanerCore(_options);

        while (true)
        {
            var readResult = loader.ReadBlock();
            if (readResult == ReadBlockResult.Error)
                throw loader.GetLastException() ?? new Exception("Unknown error");

            if (readResult == ReadBlockResult.EndOfFile)
            {
                processor.FlushLeftover(leftoverWord, writer);
                break;
            }

            var buffer = loader.GetBufferSpan();
            if (buffer.IsEmpty)
                continue;

            var result = processor.ProcessBlock(buffer, writer, readResult == ReadBlockResult.EndOfFile, leftoverWord);
            if (result.HasError)
                throw result.Error!;

            leftoverWord = result.LeftoverWord;
        }
    }

    private static string GenerateOutputPath(string inputPath)
    {
        string dir = Path.GetDirectoryName(inputPath) ?? string.Empty;
        string name = Path.GetFileNameWithoutExtension(inputPath);
        string ext = Path.GetExtension(inputPath);
        return Path.Combine(dir, $"{name}_cleaned{ext}");
    }
}

internal class TextCleanerCore
{
    private readonly TextCleanerOptions _options;
    private static readonly SearchValues<char> AllDelimiters =
        SearchValues.Create(" \t\n\r.,!?;:-()\"'[]{}<>\\/|@#$%^&*+_=`~");
    private static readonly SearchValues<char> PunctuationOnly =
        SearchValues.Create(".,!?;:-()\"'[]{}<>\\/|@#$%^&*+_=`~");

    private readonly StringBuilder _outputBuffer = new();

    public TextCleanerCore(TextCleanerOptions options)
    {
        _options = options;
    }

    public TextProcessingResult ProcessBlock(
        ReadOnlySpan<byte> buffer,
        IFileWriter writer,
        bool isLastBlock,
        string leftoverWord)
    {
        string text = Encoding.UTF8.GetString(buffer);
        var span = text.AsSpan();

        if (!string.IsNullOrEmpty(leftoverWord))
        {
            span = string.Concat(leftoverWord, text).AsSpan();
        }

        while (!span.IsEmpty)
        {
            int index = span.IndexOfAny(AllDelimiters);

            if (index == -1)
            {
                if (isLastBlock)
                {
                    AppendWordIfValid(_outputBuffer, span);
                    writer.WriteLine(_outputBuffer.ToString());
                }
                else
                {
                    return TextProcessingResult.Success(span.ToString());
                }
                break;
            }

            ReadOnlySpan<char> word = span.Slice(0, index);
            AppendWordIfValid(_outputBuffer, word);

            char delimiter = span[index];
            if (!PunctuationOnly.Contains(delimiter))
            {
                _outputBuffer.Append(delimiter);
            }
            else if (!_options.RemovePunctuation)
            {
                _outputBuffer.Append(delimiter);
            }

            span = span.Slice(index + 1);
        }

        if (_outputBuffer.Length > 0)
        {
            writer.WriteLine(_outputBuffer.ToString());
            _outputBuffer.Clear();
        }

        return TextProcessingResult.Success();
    }

    public void FlushLeftover(string leftover, IFileWriter writer)
    {
        if (!string.IsNullOrEmpty(leftover))
        {
            _outputBuffer.Clear();
            AppendWordIfValid(_outputBuffer, leftover.AsSpan());
            if (_outputBuffer.Length > 0)
            {
                writer.WriteLine(_outputBuffer.ToString());
            }
        }
    }

    private void AppendWordIfValid(StringBuilder sb, ReadOnlySpan<char> word)
    {
        if (_options.RemoveShortWords && word.Length < _options.MinWordLength)
            return;
        sb.Append(word);
    }
}