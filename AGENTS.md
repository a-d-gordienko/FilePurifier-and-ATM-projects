# AGENTS.md

## Projects

- **FilePurifier** — C#, Avalonia MVVM, очистка текстовых файлов
- **ATM** — C#, Windows Forms, симулятор банкомата

## FilePurifier

- **Solution**: `FilePurifier/FilePurifier.sln`
- **Build**: `dotnet build FilePurifier/FilePurifier.sln`
- **Tests**: xUnit + Moq. Run: `dotnet test FilePurifier/FilePurifier/FilePurifier.Tests`
- **Requires**: Avalonia for Visual Studio plugin, NuGet `CommunityToolkit.Mvvm`

## ATM

- **Build**: `dotnet build ATM.csproj`
- **Run**: `dotnet run --project ATM.csproj`
- **Tests**: xUnit. Run: `dotnet test ATM.Tests/ATM.Tests.csproj`
- **Requires**: .NET 8.0, Windows Forms

## Architecture (SOLID)

```
ATM/
├── Domain/
│     ├── Cassette.cs              — Domain models + CassetteCollection
│     └── ICassetteRepository.cs   — Interfaces (ICassetteRepository, IAtmService)
├── Infrastructure/
│     ├── InMemoryCassetteRepository.cs  — In-memory implementation
│     └── FileCassetteRepository.cs      — JSON file persistence
├── Services/
│     └── AtmService.cs            — Business logic with greedy algorithm
└── ATM.Tests/                     — 17 xUnit tests

FilePurifier/
├── FilePurifier.Core/             — Core library with interfaces
├── FilePurifier.UI/              — Avalonia MVVM
└── FilePurifier.Tests/           — 27 xUnit + Moq tests
```

## Key Features

- **ATM**: DI, interface-based design, JSON persistence
- **FilePurifier**: Block-based file reading (4096 bytes), async processing
- **Both**: SOLID principles, comprehensive tests, modern C#