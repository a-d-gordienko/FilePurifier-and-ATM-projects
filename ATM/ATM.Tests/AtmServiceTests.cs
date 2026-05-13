using ATM.Domain;
using ATM.Infrastructure;
using ATM.Services;

namespace ATM.Tests;

public class AtmServiceTests
{
    [Fact]
    public void Withdraw_InvalidAmount_ReturnsFailure()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act
        var result = service.Withdraw(new WithdrawalRequest(0, false));

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Введите сумму больше нуля", result.Message);
    }

    [Fact]
    public void Withdraw_NegativeAmount_ReturnsFailure()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act
        var result = service.Withdraw(new WithdrawalRequest(-100, false));

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void Withdraw_NotMultipleOfMinDenomination_ReturnsFailure()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act - 15 is not multiple of 10 (min denomination)
        var result = service.Withdraw(new WithdrawalRequest(15, false));

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Сумма должна быть кратна", result.Message);
    }

    [Fact]
    public void Withdraw_ValidAmount_ReturnsSuccess()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act
        var result = service.Withdraw(new WithdrawalRequest(100, false));

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Успешно", result.Message);
        Assert.NotNull(result.Dispensed);
        Assert.Contains(result.Dispensed, c => c.Denomination == 100);
    }

    [Fact]
    public void Withdraw_UnavailableAmount_ReturnsFailure()
    {
        // Arrange - создаем репозиторий и опустошаем его
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Опустошаем все кассеты (устанавливаем count = 0)
        foreach (var cassette in repository.GetAll())
        {
            repository.Add(cassette.Denomination, -cassette.Count);
        }

        // Act - пробуем снять любую сумму
        var result = service.Withdraw(new WithdrawalRequest(100, false));

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Невозможно выдать сумму", result.Message);
    }

    [Fact]
    public void Withdraw_DecreasesBalance()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);
        var initialBalance = service.GetTotalBalance();

        // Act
        service.Withdraw(new WithdrawalRequest(100, false));

        // Assert
        var newBalance = service.GetTotalBalance();
        Assert.Equal(initialBalance - 100, newBalance);
    }

    [Fact]
    public void Refill_ValidAmount_ReturnsSuccess()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act
        var (success, message) = service.Refill(100, 10);

        // Assert
        Assert.True(success);
        Assert.Empty(message);
    }

    [Fact]
    public void Refill_ExceedsMaxLimit_ReturnsFailure()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act - add more than max (500)
        var (success, message) = service.Refill(100, 600);

        // Assert
        Assert.False(success);
        Assert.Contains("Превышен лимит", message);
    }

    [Fact]
    public void GetCassettes_ReturnsAllCassettes()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act
        var cassettes = service.GetCassettes();

        // Assert
        Assert.NotEmpty(cassettes);
        Assert.All(cassettes, c => Assert.True(c.Denomination > 0 && c.Count >= 0));
    }

    [Fact]
    public void GetTotalBalance_CalculatesCorrectly()
    {
        // Arrange
        var repository = new InMemoryCassetteRepository();
        var service = new AtmService(repository);

        // Act
        var balance = service.GetTotalBalance();

        // Assert
        Assert.True(balance > 0);
    }
}