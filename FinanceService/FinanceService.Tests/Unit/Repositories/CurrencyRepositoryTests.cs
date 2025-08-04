using FinanceService.Infrastructure;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Tests.Unit.Repositories;

public class CurrencyRepositoryTests : IDisposable
{
    private readonly DbContextOptions<FinanceDbContext> _options;
    private readonly FinanceDbContext _context;
    private readonly CurrencyRepository _repository;

    public CurrencyRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new FinanceDbContext(_options);
        _repository = new CurrencyRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetAllCurrenciesAsync Tests

    [Fact]
    public async Task GetAllCurrenciesAsync_WithCurrencies_ReturnsAllCurrencies()
    {
        // Arrange
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(5);
        await _context.Currencies.AddRangeAsync(testCurrencies);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllCurrenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testCurrencies.Count, result.Count());
        
        foreach (var currency in testCurrencies)
        {
            var foundCurrency = result.FirstOrDefault(c => c.Id == currency.Id);
            Assert.NotNull(foundCurrency);
            Assert.Equal(currency.Name, foundCurrency.Name);
            Assert.Equal(currency.Name, foundCurrency.Name);
            Assert.Equal(currency.Rate, foundCurrency.Rate);
        }
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_NoCurrencies_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllCurrenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetCurrencyByNameAsync Tests

    [Fact]
    public async Task GetCurrencyByNameAsync_ExistingCurrency_ReturnsCurrency()
    {
        // Arrange
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: "USD");
        await _context.Currencies.AddAsync(testCurrency);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCurrencyByNameAsync("USD");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testCurrency.Id, result.Id);
        Assert.Equal(testCurrency.Name, result.Name);
        Assert.Equal(testCurrency.Name, result.Name);
        Assert.Equal(testCurrency.Rate, result.Rate);
    }

    [Fact]
    public async Task GetCurrencyByNameAsync_NonExistingCurrency_ReturnsNull()
    {
        // Arrange
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: "USD");
        await _context.Currencies.AddAsync(testCurrency);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCurrencyByNameAsync("EUR");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrencyByNameAsync_CaseSensitive_ReturnsCorrectCurrency()
    {
        // Arrange
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: "USD");
        await _context.Currencies.AddAsync(testCurrency);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCurrencyByNameAsync("usd");

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetCurrenciesByIdsAsync Tests

    [Fact]
    public async Task GetCurrenciesByIdsAsync_ExistingIds_ReturnsCurrencies()
    {
        // Arrange
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(3);
        await _context.Currencies.AddRangeAsync(testCurrencies);
        await _context.SaveChangesAsync();

        var currencyIds = testCurrencies.Select(c => c.Id).ToList();

        // Act
        var result = await _repository.GetCurrenciesByIdsAsync(currencyIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testCurrencies.Count, result.Count());
        
        foreach (var currency in testCurrencies)
        {
            var foundCurrency = result.FirstOrDefault(c => c.Id == currency.Id);
            Assert.NotNull(foundCurrency);
            Assert.Equal(currency.Name, foundCurrency.Name);
            Assert.Equal(currency.Name, foundCurrency.Name);
            Assert.Equal(currency.Rate, foundCurrency.Rate);
        }
    }

    [Fact]
    public async Task GetCurrenciesByIdsAsync_PartialExistingIds_ReturnsOnlyExistingCurrencies()
    {
        // Arrange
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(2);
        await _context.Currencies.AddRangeAsync(testCurrencies);
        await _context.SaveChangesAsync();

        var currencyIds = new List<int> { testCurrencies[0].Id, 999, testCurrencies[1].Id };

        // Act
        var result = await _repository.GetCurrenciesByIdsAsync(currencyIds);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        
        var resultIds = result.Select(c => c.Id).ToList();
        Assert.Contains(testCurrencies[0].Id, resultIds);
        Assert.Contains(testCurrencies[1].Id, resultIds);
        Assert.DoesNotContain(999, resultIds);
    }

    [Fact]
    public async Task GetCurrenciesByIdsAsync_NoExistingIds_ReturnsEmptyList()
    {
        // Arrange
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(2);
        await _context.Currencies.AddRangeAsync(testCurrencies);
        await _context.SaveChangesAsync();

        var currencyIds = new List<int> { 999, 1000, 1001 };

        // Act
        var result = await _repository.GetCurrenciesByIdsAsync(currencyIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCurrenciesByIdsAsync_EmptyIds_ReturnsEmptyList()
    {
        // Arrange
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(2);
        await _context.Currencies.AddRangeAsync(testCurrencies);
        await _context.SaveChangesAsync();

        var currencyIds = new List<int>();

        // Act
        var result = await _repository.GetCurrenciesByIdsAsync(currencyIds);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
} 