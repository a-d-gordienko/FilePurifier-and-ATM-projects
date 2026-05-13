using System.Text.Json;
using ATM.Domain;

namespace ATM.Infrastructure;

public class FileCassetteRepository : ICassetteRepository, IDisposable
{
    private readonly CassetteCollection _cassettes = new();
    private readonly string _filePath;

    public FileCassetteRepository(string filePath)
    {
        _filePath = filePath;
        if (File.Exists(filePath))
        {
            Load(filePath);
        }
    }

    public IReadOnlyList<Cassette> GetAll() => _cassettes.GetAll();
    public Cassette? Get(int denomination) => _cassettes.Get(denomination);

    public (bool Success, string Message) Add(int denomination, int count) =>
        _cassettes.TryAdd(denomination, count);

    public (bool Success, string Message) Refill(int denomination, int count) =>
        _cassettes.TryAdd(denomination, count);

    public int GetTotalBalance() => _cassettes.GetTotalBalance();
    public int GetMinDenomination() => _cassettes.GetMinDenomination();
    public void ApplyDispense(IReadOnlyList<Cassette> dispensed) =>
        _cassettes.ApplyDispense(dispensed);

    public void Save(string filePath)
    {
        var data = _cassettes.GetAll()
            .Select(c => new CassetteDto { Denomination = c.Denomination, Count = c.Count })
            .ToList();

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public void Load(string filePath)
    {
        if (!File.Exists(filePath)) return;

        var json = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<List<CassetteDto>>(json);

        if (data == null) return;

        foreach (var dto in data)
        {
            var existing = _cassettes.Get(dto.Denomination);
            if (existing != null)
            {
                _cassettes.TryUpdate(existing.WithCount(dto.Count));
            }
        }
    }

    public void Dispose()
    {
        Save(_filePath);
    }

    private record CassetteDto
    {
        public int Denomination { get; init; }
        public int Count { get; init; }
    }
}