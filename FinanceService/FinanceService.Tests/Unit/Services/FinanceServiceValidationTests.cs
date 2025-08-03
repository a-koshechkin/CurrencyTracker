using FinanceService.Application.Services;
using FinanceService.Domain.Interfaces;
using FinanceService.Tests.TestHelpers;
using Moq;
using Shared.Domain.Entities;
using Shared.DTOs.DTOs;

namespace FinanceService.Tests.Unit.Services;

public class FinanceServiceValidationTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserFavoriteRepository> _mockUserFavoriteRepository;
    private readonly Mock<ICurrencyRepository> _mockCurrencyRepository;
    private readonly Application.Services.FinanceService _financeService;

    public FinanceServiceValidationTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserFavoriteRepository = new Mock<IUserFavoriteRepository>();
        _mockCurrencyRepository = new Mock<ICurrencyRepository>();
        _financeService = new Application.Services.FinanceService(_mockUserRepository.Object, _mockUserFavoriteRepository.Object, _mockCurrencyRepository.Object);
    }

    #region Business Logic Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetUserFavoritesAsync_InvalidUserId_ThrowsArgumentException(int invalidUserId)
    {
        // Arrange
        _mockUserRepository.Setup(x => x.UserExistsAsync(invalidUserId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _financeService.GetUserFavoritesAsync(invalidUserId));
        
        Assert.Contains($"User with ID {invalidUserId} not found", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task AddToFavoritesAsync_InvalidUserId_ThrowsArgumentException(int invalidUserId)
    {
        // Arrange
        _mockUserRepository.Setup(x => x.UserExistsAsync(invalidUserId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _financeService.AddToFavoritesAsync(invalidUserId, "USD"));
        
        Assert.Contains($"User with ID {invalidUserId} not found", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task RemoveFromFavoritesAsync_InvalidUserId_ThrowsArgumentException(int invalidUserId)
    {
        // Arrange
        _mockUserRepository.Setup(x => x.UserExistsAsync(invalidUserId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _financeService.RemoveFromFavoritesAsync(invalidUserId, "USD"));
        
        Assert.Contains($"User with ID {invalidUserId} not found", exception.Message);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("US")]
    [InlineData("USDD")]
    public async Task AddToFavoritesAsync_InvalidCurrencyCode_ThrowsArgumentException(string invalidCurrencyCode)
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(invalidCurrencyCode))
            .ReturnsAsync((Currency)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _financeService.AddToFavoritesAsync(userId, invalidCurrencyCode));
        
        Assert.Contains($"Currency with code {invalidCurrencyCode} not found", exception.Message);
    }

    [Theory]
    [InlineData("INVALID")]
    [InlineData("US")]
    [InlineData("USDD")]
    public async Task RemoveFromFavoritesAsync_InvalidCurrencyCode_ThrowsArgumentException(string invalidCurrencyCode)
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(invalidCurrencyCode))
            .ReturnsAsync((Currency)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _financeService.RemoveFromFavoritesAsync(userId, invalidCurrencyCode));
        
        Assert.Contains($"Currency with code {invalidCurrencyCode} not found", exception.Message);
    }

    #endregion

    #region Business Rule Tests

    [Fact]
    public async Task AddToFavoritesAsync_CurrencyAlreadyInFavorites_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        var currency = TestDataBuilder.CreateTestCurrency(name: currencyCode);
        var existingFavorite = TestDataBuilder.CreateTestUserFavorite(userId, currency.Id);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync(currency);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoriteAsync(userId, currency.Id))
            .ReturnsAsync(existingFavorite);

        // Act
        var result = await _financeService.AddToFavoritesAsync(userId, currencyCode);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task RemoveFromFavoritesAsync_CurrencyNotInFavorites_ReturnsFalse()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        var currency = TestDataBuilder.CreateTestCurrency(name: currencyCode);

        _mockUserRepository.Setup(x => x.UserExistsAsync(userId))
            .ReturnsAsync(true);
        _mockCurrencyRepository.Setup(x => x.GetCurrencyByNameAsync(currencyCode))
            .ReturnsAsync(currency);
        _mockUserFavoriteRepository.Setup(x => x.GetUserFavoriteAsync(userId, currency.Id))
            .ReturnsAsync((UserFavorite)null!);

        // Act
        var result = await _financeService.RemoveFromFavoritesAsync(userId, currencyCode);

        // Assert
        Assert.False(result);
    }

    #endregion
} 