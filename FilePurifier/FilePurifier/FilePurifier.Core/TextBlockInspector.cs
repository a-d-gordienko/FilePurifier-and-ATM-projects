namespace FilePurifier.Core
{
    internal static class TextBlockInspector
    {
        private const byte HorizontalTab = 9;
        private const byte LineFeed = 10;
        private const byte CarriageReturn = 13;
        private const byte Space = 32;

        private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];

        /// <summary>
        /// Определяет, является ли блок данных текстовым (UTF-8).
        /// </summary>
        public static bool IsText(ReadOnlySpan<byte> buffer)
        {
            if (buffer.IsEmpty) return false;

            // 1. Если есть BOM, это точно текст
            if (HasUtf8Bom(buffer)) return true;

            // 2. Проверка на наличие нетекстовых байтов
            foreach (byte b in buffer)
            {
                if (b == 0) return false; // Null-байт — признак бинарного файла

                // Проверка: символ печатный или стандартный управляющий?
                if (!(b >= Space || b == HorizontalTab || b == LineFeed || b == CarriageReturn))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasUtf8Bom(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < Utf8Bom.Length) return false;
            return buffer.Slice(0, Utf8Bom.Length).SequenceEqual(Utf8Bom);
        }
    }
}
