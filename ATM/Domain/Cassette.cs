namespace ATM.Domain;

public record Cassette(int Denomination, int Count)
{
    public int TotalValue => Denomination * Count;

    public Cassette WithCount(int newCount) => new(Denomination, newCount);

    public Cassette AddCount(int additional) => new(Denomination, Count + additional);

    public Cassette SubtractCount(int subtract) => new(Denomination, Math.Max(0, Count - subtract));
}

public record WithdrawalRequest(int Amount, bool PreferExchange);

public record WithdrawalResult(
    bool Success,
    string Message,
    IReadOnlyList<Cassette> Dispensed = null!)
{
    public static WithdrawalResult Successful(IReadOnlyList<Cassette> dispensed) =>
        new(true, "Успешно", dispensed);

    public static WithdrawalResult Failed(string message) =>
        new(false, message);

    public static WithdrawalResult InvalidAmount() =>
        Failed("Введите сумму больше нуля");
}

public class CassetteCollection
{
    private readonly Dictionary<int, Cassette> _cassettes = new();
    public const int MaxCountPerDenomination = 500;

    public CassetteCollection()
    {
        InitializeDefault();
    }

    private void InitializeDefault()
    {
        var defaults = new[]
        {
            new Cassette(10, 500),
            new Cassette(50, 100),
            new Cassette(100, 50),
            new Cassette(500, 10),
            new Cassette(1000, 5),
            new Cassette(2000, 3),
            new Cassette(5000, 2)
        };

        foreach (var cassette in defaults)
        {
            _cassettes[cassette.Denomination] = cassette;
        }
    }

    public IReadOnlyList<Cassette> GetAll() => _cassettes.Values.OrderByDescending(c => c.Denomination).ToList();

    public Cassette? Get(int denomination) => _cassettes.GetValueOrDefault(denomination);

    public bool TryUpdate(Cassette cassette)
    {
        if (!_cassettes.ContainsKey(cassette.Denomination)) return false;

        if (cassette.Count < 0 || cassette.Count > MaxCountPerDenomination) return false;

        _cassettes[cassette.Denomination] = cassette;
        return true;
    }

    public (bool Success, string Message) TryAdd(int denomination, int count)
    {
        if (!_cassettes.TryGetValue(denomination, out var existing))
            return (false, $"Кассета с номиналом {denomination} не найдена");

        var newCount = existing.Count + count;
        if (newCount > MaxCountPerDenomination)
            return (false, $"Превышен лимит. Максимум {MaxCountPerDenomination} купюр");

        _cassettes[denomination] = existing.WithCount(newCount);
        return (true, string.Empty);
    }

    public void ApplyDispense(IReadOnlyList<Cassette> toDispense)
    {
        foreach (var item in toDispense)
        {
            if (_cassettes.TryGetValue(item.Denomination, out var existing))
            {
                _cassettes[item.Denomination] = existing.SubtractCount(item.Count);
            }
        }
    }

    public int GetTotalBalance() => _cassettes.Values.Sum(c => c.TotalValue);

    public int GetMinDenomination() => _cassettes.Keys.Min();
}