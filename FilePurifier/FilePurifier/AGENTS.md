# AGENTS.md

## Projects

- **FilePurifier** — C#, Avalonia MVVM, очистка текстовых файлов
- **ATM** — C#, Windows Forms, симулятор банкомата

## FilePurifier

- **Solution**: `FilePurifier/FilePurifier.sln`
- **Build**: `dotnet build FilePurifier/FilePurifier.sln`
- **Tests**: xUnit + Moq. Запуск: `dotnet test FilePurifier/FilePurifier/FilePurifier.Tests`
- **Требует**: Avalonia for Visual Studio plugin, NuGet `CommunityToolkit.Mvvm`
- **Выходной exe**: `FilePurifier/FilePurifier/FilePurifier.UI/bin/Debug/net8.0/FilePurifier.UI.exe`

## Architecture

```
FilePurifier.Core
  ├── Interfaces/
  │     ├── IFileLoader     — загрузка файла блоками
  │     ├── IFileWriter     — запись в файл
  │     └── IFilePurifierService — сервис очистки
  ├── FileLoader.cs         — реализация IFileLoader
  ├── TextFileWriter.cs    — реализация IFileWriter
  ├── TextBlockProcessor.cs — обработка блоков текста
  ├── TextCleaner.cs       — оркестратор + TextCleanerCore
  └── FilePurifierService.cs — реализация IFilePurifierService

FilePurifier.UI      — Avalonia MVVM (ViewModels, Views)
FilePurifier.Tests   — xUnit + Moq тесты
```

### SOLID Principles

- **SRP**: каждый класс отвечает за одну задачу (загрузка, запись, обработка)
- **DIP**: UI и тесты зависят от интерфейсов, не от конкретных реализаций
- **OCP**: легко добавить новые TextProcessor без изменения существующего кода
- **Тестируемость**: FileLoader, TextFileWriter, TextBlockProcessor покрыты unit-тестами

## Key Implementation Notes

- Файлы читаются блоками по 4096 байт для обработки больших файлов
- Параллельная обработка через `Task.WhenAll` (не `Parallel.ForEachAsync`)
- Интерфейсы позволяют мокать зависимости в тестах
- `TextCleanerOptions` — immutable record для конфигурации