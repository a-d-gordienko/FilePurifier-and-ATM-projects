namespace ATM.Domain;

public interface ICassetteRepository
{
    IReadOnlyList<Cassette> GetAll();
    Cassette? Get(int denomination);
    (bool Success, string Message) Add(int denomination, int count);
    (bool Success, string Message) Refill(int denomination, int count);
    void Save(string filePath);
    void Load(string filePath);
    int GetTotalBalance();
    int GetMinDenomination();
    void ApplyDispense(IReadOnlyList<Cassette> dispensed);
}

public interface IAtmService
{
    WithdrawalResult Withdraw(WithdrawalRequest request);
    (bool Success, string Message) Refill(int denomination, int count);
    int GetTotalBalance();
    IReadOnlyList<Cassette> GetCassettes();
}