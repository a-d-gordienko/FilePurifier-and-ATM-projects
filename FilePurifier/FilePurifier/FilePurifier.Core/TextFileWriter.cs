using System.Buffers;
using System.IO;
using System.Text;

namespace FilePurifier.Core;

public class TextFileWriter : IFileWriter
{
    private readonly StreamWriter _writer;

    public TextFileWriter(string filePath)
    {
        _writer = new StreamWriter(filePath, false, Encoding.UTF8);
    }

    public void WriteLine(string? line)
    {
        _writer.WriteLine(line);
    }

    public void Flush()
    {
        _writer.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}