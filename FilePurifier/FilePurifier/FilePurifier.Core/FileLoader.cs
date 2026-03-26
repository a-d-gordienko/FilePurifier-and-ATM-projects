using System;

namespace FilePurifier.Core
{
    internal class FileProcessingContext
    {
        public Stream? stream_ { get; set; }
        public byte[]? buffer_ { get; set; }
        public int bytesRead_ { get; set; }
        public bool isText_ { get; set; }
        public Exception? lastException { get; set; }
        public bool HasError => lastException != null;
    }

    public enum NextBlockError
    {
        Ok,
        EndBlock,
        Error
    }
    
    internal class FileLoader : IDisposable
    {
        public const int BufferSize = 4096;

        private readonly FileProcessingContext context_;

        private bool IsEndOfFile => context_.stream_ != null && context_.stream_.Position >= context_.stream_.Length;

        public FileLoader(string filePath)
        {
            context_ = new FileProcessingContext();            
            try
            {
                context_.stream_ = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                context_.buffer_ = new byte[BufferSize];
            }
            catch (Exception ex)
            {
                // Записываем ошибку в контекст, чтобы UI мог её показать
                context_.lastException = ex;
            }
           
        }

        public NextBlockError NextBlock()
        {
            if (context_.HasError || context_.stream_ == null)
                return NextBlockError.Error;

            try
            {
                // 1. Читаем стандартный блок
                int read = context_.stream_.Read(context_.buffer_!, 0, BufferSize);
                if (read == 0) return NextBlockError.EndBlock;

                // 2. Проверяем, не разрезали ли мы символ UTF-8 в конце
                int validLength = GetUtf8SafeLength(context_.buffer_!.AsSpan(0, read));

                if (validLength < read)
                {
                    // Откатываем позицию стрима назад на "откушенные" байты
                    int diff = read - validLength;
                    context_.stream_.Position -= diff;
                    read = validLength;
                }

                context_.bytesRead_ = read;
                return IsEndOfFile ? NextBlockError.EndBlock : NextBlockError.Ok;
            }
            catch (Exception ex)
            {
                context_.lastException = ex;
                return NextBlockError.Error;
            }
        }

        // Вспомогательный метод для поиска границы целого символа UTF-8
        private int GetUtf8SafeLength(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length == 0) return 0;

            // В UTF-8 многобайтовые символы начинаются с байта 11xxxxxx
            // Продолжающиеся байты выглядят как 10xxxxxx
            // Мы идем с конца и ищем начало символа
            for (int i = buffer.Length - 1; i >= Math.Max(0, buffer.Length - 4); i--)
            {
                byte b = buffer[i];

                // 1. Если это ASCII (0xxxxxxx), то всё хорошо, это конец символа
                if ((b & 0x80) == 0) return buffer.Length;

                // 2. Если это начало многобайтового символа (11xxxxxx)
                if ((b & 0xC0) == 0xC0)
                {
                    // Вычисляем ожидаемую длину символа по первым битам
                    int expectedLen = (b & 0xE0) == 0xC0 ? 2 :
                                      (b & 0xF0) == 0xE0 ? 3 :
                                      (b & 0xF8) == 0xF0 ? 4 : 1;

                    // Если символ помещается целиком до конца буфера - всё ок
                    if (buffer.Length - i >= expectedLen) return buffer.Length;

                    // Если не помещается - обрезаем буфер до этого байта
                    return i;
                }

                // 3. Если это продолжающийся байт (10xxxxxx), идем дальше назад
            }
            return buffer.Length;
        }


        public Exception? GetLastException() => context_.lastException;

        public ReadOnlySpan<byte> GetBufferSpan()
        {
            if (context_.buffer_ == null || context_.bytesRead_ <= 0)
                return ReadOnlySpan<byte>.Empty;

            // Возвращает "окно" в массив ровно по размеру прочитанных данных
            return context_.buffer_.AsSpan(0, context_.bytesRead_);
        }

        // Реализация IDisposable
        public void Dispose()
        {
            // Закрываем поток внутри контекста
            context_.stream_?.Dispose();
        }

        public FileProcessingContext GetContext()
        {
            return context_;
        }
    }
}
