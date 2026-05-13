using System.Buffers;
using System.IO;
using System.Text;

namespace FilePurifier.Core;

public class TextBlockProcessor : ITextProcessor
{
    private static readonly SearchValues<char> AllDelimiters =
        SearchValues.Create(" \t\n\r.,!?;:-()\"'[]{}<>\\/|@#$%^&*+_=`~");

    private static readonly SearchValues<char> PunctuationOnly =
        SearchValues.Create(".,!?;:-()\"'[]{}<>\\/|@#$%^&*+_=`~");

    public TextProcessingResult ProcessBlock(
        ReadOnlySpan<byte> buffer,
        IFileWriter writer,
        bool isLastBlock)
    {
        string text = Encoding.UTF8.GetString(buffer);
        return ProcessText(text, writer, isLastBlock);
    }

    public TextProcessingResult ProcessText(
        string text,
        IFileWriter writer,
        bool isLastBlock,
        string leftoverWord = "")
    {
        var output = new StringBuilder(text.Length);
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
                    writer.WriteLine(span.ToString());
                }
                else
                {
                    return TextProcessingResult.Success(span.ToString());
                }
                break;
            }

            ReadOnlySpan<char> word = span.Slice(0, index);
            if (word.Length > 0)
            {
                writer.WriteLine(word.ToString());
            }

            char delimiter = span[index];
            if (!PunctuationOnly.Contains(delimiter))
            {
                writer.WriteLine(delimiter.ToString());
            }

            span = span.Slice(index + 1);
        }

        writer.Flush();
        return TextProcessingResult.Success();
    }
}