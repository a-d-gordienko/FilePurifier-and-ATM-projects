namespace ATM.Infrastructure;

using ATM.Domain;

public class InMemoryCassetteRepository : ICassetteRepository
{
    private readonly CassetteCollection _cassettes = new();

    public IReadOnlyList<Cassette> GetAll() => _cassettes.GetAll();
    public Cassette? Get(int denomination) => _cassettes.Get(denomination);

    public (bool Success, string Message) Add(int denomination, int count) =>
        _cassettes.TryAdd(denomination, count);

    public (bool Success, string Message) Refill(int denomination, int count) =>
        _cassettes.TryAdd(denomination, count);

    public void Save(string filePath) =>
        throw new NotSupportedException("Use FileCassetteRepository for persistence");

    public void Load(string filePath) =>
        throw new NotSupportedException("Use FileCassetteRepository for persistence");

    public int GetTotalBalance() => _cassettes.GetTotalBalance();
    public int GetMinDenomination() => _cassettes.GetMinDenomination();
    public void ApplyDispense(IReadOnlyList<Cassette> dispensed) =>
        _cassettes.ApplyDispense(dispensed);
}