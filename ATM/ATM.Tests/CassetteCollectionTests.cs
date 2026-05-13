using ATM.Domain;
using ATM.Infrastructure;

namespace ATM.Tests;

public class CassetteCollectionTests
{
    [Fact]
    public void GetMinDenomination_ReturnsSmallest()
    {
        // Arrange
        var collection = new CassetteCollection();

        // Act
        var minDenom = collection.GetMinDenomination();

        // Assert
        Assert.Equal(10, minDenom);
    }

    [Fact]
    public void GetTotalBalance_CalculatesCorrectly()
    {
        // Arrange
        var collection = new CassetteCollection();
        // Default: 10*500 + 50*100 + 100*50 + 500*10 + 1000*5 + 2000*3 + 5000*2
        // = 5000 + 5000 + 5000 + 5000 + 5000 + 6000 + 10000 = 41000

        // Act
        var balance = collection.GetTotalBalance();

        // Assert
        Assert.Equal(41000, balance);
    }

    [Fact]
    public void TryAdd_ValidAmount_UpdatesCount()
    {
        // Arrange
        var collection = new CassetteCollection();
        var initial = collection.Get(100)?.Count ?? 0;

        // Act
        var (success, _) = collection.TryAdd(100, 10);

        // Assert
        Assert.True(success);
        Assert.Equal(initial + 10, collection.Get(100)?.Count);
    }

    [Fact]
    public void TryAdd_ExceedsMax_ReturnsFalse()
    {
        // Arrange
        var collection = new CassetteCollection();

        // Act
        var (success, message) = collection.TryAdd(100, 600);

        // Assert
        Assert.False(success);
        Assert.Contains("лимит", message);
    }

    [Fact]
    public void ApplyDispense_SubtractsFromCassettes()
    {
        // Arrange
        var collection = new CassetteCollection();
        var toDispense = new List<Cassette> { new(100, 5) };

        // Act
        collection.ApplyDispense(toDispense);

        // Assert
        Assert.Equal(45, collection.Get(100)?.Count); // 50 - 5 = 45
    }
}