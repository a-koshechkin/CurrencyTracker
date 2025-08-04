using FinanceService.Infrastructure;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Tests.Unit.Repositories;

public class UserFavoriteRepositoryTests : IDisposable
{
    private readonly DbContextOptions<FinanceDbContext> _options;
    private readonly FinanceDbContext _context;
    private readonly UserFavoriteRepository _repository;

    public UserFavoriteRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new FinanceDbContext(_options);
        _repository = new UserFavoriteRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region GetUserFavoritesAsync Tests

    [Fact]
    public async Task GetUserFavoritesAsync_UserWithFavorites_ReturnsFavorites()
    {
        // Arrange
        var userId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(3);
        var testFavorites = TestDataBuilder.CreateTestUserFavorites(userId, testCurrencies.Select(c => c.Id).ToList());

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddRangeAsync(testCurrencies);
        await _context.UserFavorites.AddRangeAsync(testFavorites);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testFavorites.Count, result.Count());
        
        foreach (var favorite in testFavorites)
        {
            var foundFavorite = result.FirstOrDefault(f => f.UserId == favorite.UserId && f.CurrencyId == favorite.CurrencyId);
            Assert.NotNull(foundFavorite);
            Assert.Equal(favorite.UserId, foundFavorite.UserId);
            Assert.Equal(favorite.CurrencyId, foundFavorite.CurrencyId);
        }
    }

    [Fact]
    public async Task GetUserFavoritesAsync_UserWithoutFavorites_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserFavoritesAsync_NonExistingUser_ReturnsEmptyList()
    {
        // Arrange
        var userId = 999;

        // Act
        var result = await _repository.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetUserFavoriteAsync Tests

    [Fact]
    public async Task GetUserFavoriteAsync_ExistingFavorite_ReturnsFavorite()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);
        var testFavorite = TestDataBuilder.CreateTestUserFavorite(userId, currencyId);

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.UserFavorites.AddAsync(testFavorite);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserFavoriteAsync(userId, currencyId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testFavorite.UserId, result.UserId);
        Assert.Equal(testFavorite.CurrencyId, result.CurrencyId);
    }

    [Fact]
    public async Task GetUserFavoriteAsync_NonExistingFavorite_ReturnsNull()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserFavoriteAsync(userId, currencyId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserFavoriteAsync_DifferentUserFavorite_ReturnsNull()
    {
        // Arrange
        var userId1 = 1;
        var userId2 = 2;
        var currencyId = 1;
        var testUser1 = TestDataBuilder.CreateTestUser(userId1);
        var testUser2 = TestDataBuilder.CreateTestUser(userId2);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);
        var testFavorite = TestDataBuilder.CreateTestUserFavorite(userId2, currencyId);

        await _context.Users.AddRangeAsync([testUser1, testUser2]);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.UserFavorites.AddAsync(testFavorite);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserFavoriteAsync(userId1, currencyId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region AddUserFavoriteAsync Tests

    [Fact]
    public async Task AddUserFavoriteAsync_ValidFavorite_AddsToDatabase()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.SaveChangesAsync();

        // Act
        await _repository.AddUserFavoriteAsync(userId, currencyId);

        // Assert
        var addedFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencyId);
        
        Assert.NotNull(addedFavorite);
        Assert.Equal(userId, addedFavorite.UserId);
        Assert.Equal(currencyId, addedFavorite.CurrencyId);
    }

    [Fact]
    public async Task AddUserFavoriteAsync_DuplicateFavorite_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);
        var existingFavorite = TestDataBuilder.CreateTestUserFavorite(userId, currencyId);

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.UserFavorites.AddAsync(existingFavorite);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _repository.AddUserFavoriteAsync(userId, currencyId));
    }

    #endregion

    #region RemoveUserFavoriteAsync Tests

    [Fact]
    public async Task RemoveUserFavoriteAsync_ExistingFavorite_RemovesFromDatabase()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);
        var testFavorite = TestDataBuilder.CreateTestUserFavorite(userId, currencyId);

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.UserFavorites.AddAsync(testFavorite);
        await _context.SaveChangesAsync();

        var existingFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencyId);
        Assert.NotNull(existingFavorite);

        // Act
        await _repository.RemoveUserFavoriteAsync(userId, currencyId);

        // Assert
        var removedFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencyId);
        
        Assert.Null(removedFavorite);
    }

    [Fact]
    public async Task RemoveUserFavoriteAsync_NonExistingFavorite_DoesNothing()
    {
        // Arrange
        var userId = 1;
        var currencyId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(currencyId);

        await _context.Users.AddAsync(testUser);
        await _context.Currencies.AddAsync(testCurrency);
        await _context.SaveChangesAsync();

        // Act
        await _repository.RemoveUserFavoriteAsync(userId, currencyId);

        // Assert
        var favoritesCount = await _context.UserFavorites.CountAsync();
        Assert.Equal(0, favoritesCount);
    }

    #endregion
} 