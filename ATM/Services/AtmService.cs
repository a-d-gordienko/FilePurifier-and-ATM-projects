using ATM.Domain;

namespace ATM.Services;

public class AtmService : IAtmService
{
    private readonly ICassetteRepository _cassettes;

    public AtmService(ICassetteRepository cassettes)
    {
        _cassettes = cassettes;
    }

    public WithdrawalResult Withdraw(WithdrawalRequest request)
    {
        if (request.Amount <= 0)
            return WithdrawalResult.InvalidAmount();

        int minDenomination = _cassettes.GetMinDenomination();
        if (request.Amount % minDenomination != 0)
            return WithdrawalResult.Failed($"Сумма должна быть кратна {minDenomination} руб.");

        var dispensed = CalculateDispense(request.Amount, request.PreferExchange);

        if (dispensed.Count == 0)
            return WithdrawalResult.Failed("Невозможно выдать сумму");

        _cassettes.ApplyDispense(dispensed);

        return WithdrawalResult.Successful(dispensed);
    }

    public (bool Success, string Message) Refill(int denomination, int count)
    {
        return _cassettes.Add(denomination, count);
    }

    public int GetTotalBalance() => _cassettes.GetTotalBalance();

    public IReadOnlyList<Cassette> GetCassettes() => _cassettes.GetAll();

    private IReadOnlyList<Cassette> CalculateDispense(int amount, bool preferExchange)
    {
        var available = _cassettes.GetAll()
            .Where(c => c.Count > 0)
            .OrderByDescending(c => c.Denomination)
            .ToList();

        var result = new List<Cassette>();
        int remaining = amount;

        for (int i = 0; i < available.Count; i++)
        {
            var cassette = available[i];
            int countNeeded = remaining / cassette.Denomination;
            int actualToGive = Math.Min(countNeeded, cassette.Count);

            if (preferExchange && actualToGive > 0 && remaining == amount)
            {
                actualToGive--;
            }

            if (actualToGive > 0)
            {
                remaining -= actualToGive * cassette.Denomination;
                result.Add(new Cassette(cassette.Denomination, actualToGive));
            }

            if (remaining == 0) break;
        }

        return remaining == 0 ? result : new List<Cassette>();
    }
}