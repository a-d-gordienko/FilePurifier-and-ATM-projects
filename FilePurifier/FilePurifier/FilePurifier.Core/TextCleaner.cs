using System.Buffers;
using System.Text;

namespace FilePurifier.Core
{
    internal class TextCleaner
    {
        private readonly bool _removeShortWords;
        private readonly int _minWordLength;
        private readonly bool _removePunctuation;
        private FileProcessingContext? context_;

        // Все, что обрывает слово
        private static readonly SearchValues<char> AllDelimiters =
            SearchValues.Create(" \t\n\r.,!?;:-()\"'[]{}<>\\/|@#$%^&*+_=`~");

        // Только те, которые мы считаем пунктуацией для удаления
        private static readonly SearchValues<char> PunctuationOnly =
            SearchValues.Create(".,!?;:-()\"'[]{}<>\\/|@#$%^&*+_=`~");

        // Хранит часть слова, разорванного между блоками
        private string _leftoverWord = string.Empty;

        public TextCleaner(bool removeShortWords, int minWordLength, bool removePunctuation)
        {
            _removeShortWords = removeShortWords;
            _minWordLength = minWordLength;
            _removePunctuation = removePunctuation;           
        }

        public void Process(string inputPath)
        {
            string outputPath = GenerateOutputPath(inputPath);

            using var loader = new FileLoader(inputPath);
            context_ = loader.GetContext();
            if (loader.GetLastException() != null)
            {
                throw context_.lastException!;
            }
            // 1. Проверка первого блока
            var status = loader.NextBlock();
            if (status == NextBlockError.Error)
            {
                if (loader.GetLastException() == null)
                {
                    context_.lastException = new Exception("Unknown error during file loading.");                   
                }
                throw context_.lastException!;
            }
                

            if (!TextBlockInspector.IsText(loader.GetBufferSpan()))
            {
                context_.lastException = new InvalidOperationException($"Файл {inputPath} не является текстовым.");
                throw context_.lastException;
            }
                

            // 2. Основной цикл обработки
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                while (true)
                {
                    bool isLastBlock = (status == NextBlockError.EndBlock);
                    ProcessBlock(loader.GetBufferSpan(), writer, isLastBlock);

                    if (isLastBlock) break;

                    status = loader.NextBlock();
                    if (status == NextBlockError.Error)
                        throw loader.GetLastException()!;
                }
            }
        }

        private void ProcessBlock(ReadOnlySpan<byte> buffer, StreamWriter writer, bool isLastBlock)
        {
            // Декодируем байты. Благодаря FileLoader, здесь всегда целые символы UTF-8
            string text = Encoding.UTF8.GetString(buffer);
            ReadOnlySpan<char> span = text.AsSpan();

            // Склеиваем с остатком слова из предыдущего блока (если он был)
            if (!string.IsNullOrEmpty(_leftoverWord))
            {
                span = string.Concat(_leftoverWord, text).AsSpan();
                _leftoverWord = string.Empty;
            }

            // Буфер для накопления очищенного текста перед записью в файл
            var outputBatch = new StringBuilder(span.Length);

            while (!span.IsEmpty)
            {
                // Находим индекс ЛЮБОГО разделителя (пробел, знак препинания, перенос строки)
                int index = span.IndexOfAny(AllDelimiters);

                if (index == -1)
                {
                    // Разделителей до конца блока нет
                    if (isLastBlock)
                    {
                        AppendWordIfValid(outputBatch, span);
                    }
                    else
                    {
                        // Сохраняем "хвост" для склейки со следующим блоком
                        _leftoverWord = span.ToString();
                    }
                    break;
                }

                // 1. Выделяем найденное слово (текст ДО разделителя)
                ReadOnlySpan<char> word = span.Slice(0, index);
                if (word.Length > 0)
                {
                    AppendWordIfValid(outputBatch, word);
                }

                // 2. Обрабатываем сам разделитель (символ ПО индексу)
                char delimiter = span[index];

                // Если это пунктуация (.,!?) — удаляем или оставляем по флагу
                if (PunctuationOnly.Contains(delimiter))
                {
                    if (!_removePunctuation)
                        outputBatch.Append(delimiter);
                }
                else
                {
                    // Если это пробел, таб или \n — ВСЕГДА оставляем, чтобы сохранить структуру
                    outputBatch.Append(delimiter);
                }

                // 3. Отрезаем обработанную часть и продолжаем поиск в остатке
                span = span.Slice(index + 1);
            }

            // Записываем весь обработанный блок в файл за один раз
            writer.Write(outputBatch.ToString());
        }


        private void AppendWordIfValid(StringBuilder sb, ReadOnlySpan<char> word)
        {
            // Проверка: если удаление включено и слово слишком короткое — ничего не делаем
            if (_removeShortWords && word.Length < _minWordLength)
            {
                return;
            }

            // Если слово проходит фильтр — добавляем его в буфер
            sb.Append(word);
        }

        private string GenerateOutputPath(string inputPath)
        {
            string dir = Path.GetDirectoryName(inputPath) ?? string.Empty;
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string ext = Path.GetExtension(inputPath);
            return Path.Combine(dir, $"{fileName}_cleaned{ext}");
        }
    }
}
