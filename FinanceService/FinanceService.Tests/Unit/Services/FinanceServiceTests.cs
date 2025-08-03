using FinanceService.Domain.Interfaces;
using FinanceService.Tests.TestHelpers;
using Moq;
using Shared.Domain.Entities;

namespace FinanceService.Tests.Unit.Services;

public class FinanceServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserFavoriteRepository> _mockUserFavoriteRepository;
    private readonly Mock<ICurrencyRepository> _mockCurrencyRepository;
    private readonly Application.Services.FinanceService _financeService;

    public FinanceServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserFavoriteRepository = new Mock<IUserFavoriteRepository>();
        _mockCurrencyRepository = new Mock<ICurrencyRepository>();
        _financeService = new Application.Services.FinanceService(
            _mockUserRepository.Object,
            _mockUserFavoriteRepository.Object,
            _mockCurrencyRepository.Object);
    }

    #region GetUserFavoritesAsync Tests

    [Fact]
    public async Task GetUserFavoritesAsync_ValidUserIdWithFavorites_ReturnsUserFavorites()
    {
        // Arrange
        var userId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(3);
        var testFavorites = TestDataBuilder.CreateTestUserFavorites(userId, testCurrencies.Select(c => c.Id).ToList());

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoritesAsync(userId))
            .ReturnsAsync(testFavorites);
        _mockCurrencyRepository.Setup(x => x.GetCurrenciesByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(testCurrencies);

        // Act
        var result = await _financeService.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testCurrencies.Count, result.FavoriteCurrencies.Count);
        
        for (int i = 0; i < testCurrencies.Count; i++)
        {
            Assert.Equal(testCurrencies[i].Name, result.FavoriteCurrencies[i].CurrencyName);
            Assert.Equal(testCurrencies[i].Rate, result.FavoriteCurrencies[i].Rate);
        }

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoritesAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrenciesByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
    }

    [Fact]
    public async Task GetUserFavoritesAsync_ValidUserIdNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        var testUser = TestDataBuilder.CreateTestUser(userId);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoritesAsync(userId))
            .ReturnsAsync(new List<UserFavorite>());

        // Act
        var result = await _financeService.GetUserFavoritesAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.FavoriteCurrencies);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoritesAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrenciesByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task GetUserFavoritesAsync_UserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var userId = 999;

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _financeService.GetUserFavoritesAsync(userId));
        
        Assert.Equal($"User with ID {userId} not found", exception.Message);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoritesAsync(It.IsAny<int>()), Times.Never);
        _mockCurrencyRepository.Verify(x => x.GetCurrenciesByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    #endregion

    #region GetAllCurrenciesAsync Tests

    [Fact]
    public async Task GetAllCurrenciesAsync_ValidRequest_ReturnsAllCurrencies()
    {
        // Arrange
        var testCurrencies = TestDataBuilder.CreateTestCurrencies(5);

        _mockCurrencyRepository.Setup(x => x.GetAllCurrenciesAsync())
            .ReturnsAsync(testCurrencies);

        // Act
        var result = await _financeService.GetAllCurrenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testCurrencies.Count, result.Count);
        
        for (int i = 0; i < testCurrencies.Count; i++)
        {
            Assert.Equal(testCurrencies[i].Name, result[i].Name);
            Assert.Equal(testCurrencies[i].Rate, result[i].Rate);
        }

        _mockCurrencyRepository.Verify(x => x.GetAllCurrenciesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllCurrenciesAsync_NoCurrencies_ReturnsEmptyList()
    {
        // Arrange
        _mockCurrencyRepository.Setup(x => x.GetAllCurrenciesAsync())
            .ReturnsAsync(new List<Currency>());

        // Act
        var result = await _financeService.GetAllCurrenciesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _mockCurrencyRepository.Verify(x => x.GetAllCurrenciesAsync(), Times.Once);
    }

    #endregion

    #region AddToFavoritesAsync Tests

    [Fact]
    public async Task AddToFavoritesAsync_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: currencyCode);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync(testCurrency);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoriteAsync(userId, testCurrency.Id))
            .ReturnsAsync((UserFavorite?)null);
        _mockUserFavoriteRepository.Setup(x => x.AddUserFavoriteAsync(userId, testCurrency.Id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _financeService.AddToFavoritesAsync(userId, currencyCode);

        // Assert
        Assert.True(result);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(currencyCode), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(userId, testCurrency.Id), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.AddUserFavoriteAsync(userId, testCurrency.Id), Times.Once);
    }

    [Fact]
    public async Task AddToFavoritesAsync_UserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var userId = 999;
        var currencyCode = "USD";

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _financeService.AddToFavoritesAsync(userId, currencyCode));
        
        Assert.Equal($"User with ID {userId} not found", exception.Message);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(It.IsAny<string>()), Times.Never);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockUserFavoriteRepository.Verify(x => x.AddUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AddToFavoritesAsync_CurrencyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "INVALID";
        var testUser = TestDataBuilder.CreateTestUser(userId);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync((Currency?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _financeService.AddToFavoritesAsync(userId, currencyCode));
        
        Assert.Equal($"Currency with code {currencyCode} not found", exception.Message);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(currencyCode), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockUserFavoriteRepository.Verify(x => x.AddUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AddToFavoritesAsync_CurrencyAlreadyInFavorites_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: currencyCode);
        var existingFavorite = TestDataBuilder.CreateTestUserFavorite(userId, testCurrency.Id);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync(testCurrency);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoriteAsync(userId, testCurrency.Id))
            .ReturnsAsync(existingFavorite);

        // Act
        var result = await _financeService.AddToFavoritesAsync(userId, currencyCode);

        // Assert
        Assert.False(result);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(currencyCode), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(userId, testCurrency.Id), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.AddUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region RemoveFromFavoritesAsync Tests

    [Fact]
    public async Task RemoveFromFavoritesAsync_ValidRequest_ReturnsTrue()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: currencyCode);
        var existingFavorite = TestDataBuilder.CreateTestUserFavorite(userId, testCurrency.Id);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync(testCurrency);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoriteAsync(userId, testCurrency.Id))
            .ReturnsAsync(existingFavorite);
        _mockUserFavoriteRepository.Setup(x => x.RemoveUserFavoriteAsync(userId, testCurrency.Id))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _financeService.RemoveFromFavoritesAsync(userId, currencyCode);

        // Assert
        Assert.True(result);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(currencyCode), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(userId, testCurrency.Id), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.RemoveUserFavoriteAsync(userId, testCurrency.Id), Times.Once);
    }

    [Fact]
    public async Task RemoveFromFavoritesAsync_UserNotFound_ThrowsArgumentException()
    {
        // Arrange
        var userId = 999;
        var currencyCode = "USD";

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _financeService.RemoveFromFavoritesAsync(userId, currencyCode));
        
        Assert.Equal($"User with ID {userId} not found", exception.Message);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(It.IsAny<string>()), Times.Never);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockUserFavoriteRepository.Verify(x => x.RemoveUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoveFromFavoritesAsync_CurrencyNotFound_ThrowsArgumentException()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "INVALID";
        var testUser = TestDataBuilder.CreateTestUser(userId);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync((Currency?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _financeService.RemoveFromFavoritesAsync(userId, currencyCode));
        
        Assert.Equal($"Currency with code {currencyCode} not found", exception.Message);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(currencyCode), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        _mockUserFavoriteRepository.Verify(x => x.RemoveUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task RemoveFromFavoritesAsync_CurrencyNotInFavorites_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        var testUser = TestDataBuilder.CreateTestUser(userId);
        var testCurrency = TestDataBuilder.CreateTestCurrency(name: currencyCode);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync(testCurrency);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoriteAsync(userId, testCurrency.Id))
            .ReturnsAsync((UserFavorite?)null);

        // Act
        var result = await _financeService.RemoveFromFavoritesAsync(userId, currencyCode);

        // Assert
        Assert.False(result);

        _mockUserRepository.Verify(x => x.UserExistsAsync(userId), Times.Once);
        _mockCurrencyRepository.Verify(x => x.GetCurrencyByNameAsync(currencyCode), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.GetUserFavoriteAsync(userId, testCurrency.Id), Times.Once);
        _mockUserFavoriteRepository.Verify(x => x.RemoveUserFavoriteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    #endregion
} 