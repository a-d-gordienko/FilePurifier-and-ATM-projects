using ATM.Domain;
using ATM.Infrastructure;

namespace ATM.Tests;

public class FileCassetteRepositoryTests : IDisposable
{
    private readonly string _testDir;

    public FileCassetteRepositoryTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDir);
    }

    [Fact]
    public void SaveAndLoad_PreservesData()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "cassettes.json");

        using (var repository = new FileCassetteRepository(filePath))
        {
            repository.Refill(100, 10);
            repository.Save(filePath);
        }

        // Act
        using (var repository = new FileCassetteRepository(filePath))
        {
            var cassette = repository.Get(100);

            // Assert
            Assert.NotNull(cassette);
        }
    }

    [Fact]
    public void Load_NonExistentFile_CreatesDefault()
    {
        // Arrange
        string filePath = Path.Combine(_testDir, "nonexistent.json");

        // Act
        using var repository = new FileCassetteRepository(filePath);
        var balance = repository.GetTotalBalance();

        // Assert
        Assert.True(balance > 0); // Default values loaded
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, true);
    }
}