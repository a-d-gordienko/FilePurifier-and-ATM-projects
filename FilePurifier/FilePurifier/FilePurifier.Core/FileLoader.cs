using System.Buffers;
using System.Text;

namespace FilePurifier.Core;

public class FileLoader : IFileLoader
{
    public const int BufferSize = 4096;

    private readonly Stream? _stream;
    private readonly byte[] _buffer = new byte[BufferSize];
    private int _bytesRead;
    private bool _hasError;
    private Exception? _lastException;

    public FileLoader(string filePath)
    {
        _stream = TryOpenFile(filePath, out var exception);
        if (exception != null)
        {
            _hasError = true;
            _lastException = exception;
        }
    }

    private static Stream? TryOpenFile(string filePath, out Exception? exception)
    {
        try
        {
            exception = null;
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception ex)
        {
            exception = ex;
            return null;
        }
    }

    public bool HasError => _hasError;
    public Exception? GetLastException() => _lastException;

    public ReadBlockResult ReadBlock()
    {
        if (_hasError || _stream == null)
            return ReadBlockResult.Error;

        try
        {
            int read = _stream.Read(_buffer.AsSpan(0, BufferSize));
            if (read == 0)
                return ReadBlockResult.EndOfFile;

            int validLength = GetUtf8SafeLength(_buffer.AsSpan(0, read));
            if (validLength < read)
            {
                _stream.Position -= read - validLength;
                read = validLength;
            }

            _bytesRead = read;
            return _stream.Position >= _stream.Length
                ? ReadBlockResult.EndOfFile
                : ReadBlockResult.Ok;
        }
        catch (Exception ex)
        {
            _hasError = true;
            _lastException = ex;
            return ReadBlockResult.Error;
        }
    }

    public ReadOnlySpan<byte> GetBufferSpan()
    {
        if (_bytesRead <= 0)
            return ReadOnlySpan<byte>.Empty;
        return _buffer.AsSpan(0, _bytesRead);
    }

    private static int GetUtf8SafeLength(ReadOnlySpan<byte> buffer)
    {
        if (buffer.IsEmpty)
            return 0;

        for (int i = buffer.Length - 1; i >= Math.Max(0, buffer.Length - 4); i--)
        {
            byte b = buffer[i];

            if ((b & 0x80) == 0)
                return buffer.Length;

            if ((b & 0xC0) == 0xC0)
            {
                int expectedLen = (b & 0xE0) == 0xC0 ? 2 :
                                  (b & 0xF0) == 0xE0 ? 3 :
                                  (b & 0xF8) == 0xF0 ? 4 : 1;

                if (buffer.Length - i >= expectedLen)
                    return buffer.Length;

                return i;
            }
        }
        return buffer.Length;
    }

    public void Dispose()
    {
        _stream?.Dispose();
    }
}