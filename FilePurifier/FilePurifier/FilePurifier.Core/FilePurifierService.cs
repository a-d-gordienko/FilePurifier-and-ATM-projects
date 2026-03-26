using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilePurifier.Core
{
    // Публичный интерфейс для UI проекта
    public class FilePurifierService
    {
        private readonly bool _removeWords;
        private readonly int _minWordLength;
        private readonly bool _removePunctuation;

        public FilePurifierService(bool removeWords, int minWordLength, bool removePunctuation)
        {
            _removeWords = removeWords;
            _minWordLength = minWordLength;
            _removePunctuation = removePunctuation;
        }

        /// <summary>
        /// Асинхронно обрабатывает список файлов один за другим.
        /// </summary>
        public async Task ProcessFilesAsync(IEnumerable<string> filePaths, IProgress<string>? progress = null)
        {
            // Ограничиваем параллелизм количеством ядер процессора (опционально)
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

            await Parallel.ForEachAsync(filePaths, options, async (path, token) =>
            {
                // Каждая итерация запускается в своем потоке из пула
                await Task.Run(() =>
                {
                    var cleaner = new TextCleaner(_removeWords, _minWordLength, _removePunctuation);
                    cleaner.Process(path);
                }, token);

                // Уведомляем UI (Report безопасен для вызова из разных потоков)
                progress?.Report(path);
            });
        }
    }
}
