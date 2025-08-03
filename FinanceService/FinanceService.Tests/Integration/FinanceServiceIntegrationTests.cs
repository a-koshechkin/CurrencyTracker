using FinanceService.API.Controllers;
using FinanceService.Infrastructure;
using FinanceService.Infrastructure.Repositories;
using FinanceService.Tests.TestHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs.DTOs;
using System.Security.Claims;

namespace FinanceService.Tests.Integration;

public class FinanceServiceIntegrationTests : IDisposable
{
    private readonly DbContextOptions<FinanceDbContext> _options;
    private readonly FinanceDbContext _context;
    private readonly FinanceController _controller;
    private readonly Application.Services.FinanceService _financeService;
    private readonly UserRepository _userRepository;
    private readonly CurrencyRepository _currencyRepository;
    private readonly UserFavoriteRepository _userFavoriteRepository;
    private readonly HttpContext _httpContext;

    public FinanceServiceIntegrationTests()
    {
        _options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new FinanceDbContext(_options);
        
        _userRepository = new UserRepository(_context);
        _currencyRepository = new CurrencyRepository(_context);
        _userFavoriteRepository = new UserFavoriteRepository(_context);
        
        _financeService = new Application.Services.FinanceService(
            _userRepository, 
            _userFavoriteRepository, 
            _currencyRepository);
        
        _controller = new FinanceController(_financeService);
        
        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SetupUserClaims(int userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _httpContext.User = principal;
    }

    private async Task<Shared.Domain.Entities.User> CreateTestUserInDatabase(int userId)
    {
        var user = TestDataBuilder.CreateTestUser(userId);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    private async Task<List<Shared.Domain.Entities.Currency>> CreateTestCurrenciesInDatabase(int count = 5)
    {
        var currencies = TestDataBuilder.CreateTestCurrencies(count);
        await _context.Currencies.AddRangeAsync(currencies);
        await _context.SaveChangesAsync();
        return currencies;
    }

    #region GetUserFavorites Integration Tests

    [Fact]
    public async Task GetUserFavorites_Integration_ValidUserWithFavorites_ReturnsFavorites()
    {
        // Arrange
        var userId = 1;
        var user = await CreateTestUserInDatabase(userId);
        var currencies = await CreateTestCurrenciesInDatabase(3);
        var favorites = TestDataBuilder.CreateTestUserFavorites(userId, currencies.Select(c => c.Id).ToList());
        
        await _context.UserFavorites.AddRangeAsync(favorites);
        await _context.SaveChangesAsync();
        
        SetupUserClaims(userId);

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User favorites retrieved successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.FavoriteCurrencies);
        Assert.Equal(currencies.Count, response.Data.FavoriteCurrencies.Count);
        
        for (int i = 0; i < currencies.Count; i++)
        {
            Assert.Equal(currencies[i].Name, response.Data.FavoriteCurrencies[i].CurrencyName);
            Assert.Equal(currencies[i].Rate, response.Data.FavoriteCurrencies[i].Rate);
        }
    }

    [Fact]
    public async Task GetUserFavorites_Integration_ValidUserNoFavorites_ReturnsEmptyList()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        SetupUserClaims(userId);

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User favorites retrieved successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.FavoriteCurrencies);
        Assert.Empty(response.Data.FavoriteCurrencies);
    }

    [Fact]
    public async Task GetUserFavorites_Integration_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        SetupUserClaims(userId);

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal($"User with ID {userId} not found", response.Message);
    }

    #endregion

    #region GetAllCurrencies Integration Tests

    [Fact]
    public async Task GetAllCurrencies_Integration_WithCurrencies_ReturnsAllCurrencies()
    {
        // Arrange
        var currencies = await CreateTestCurrenciesInDatabase(5);

        // Act
        var result = await _controller.GetAllCurrencies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<List<CurrencyResponse>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("All currencies retrieved successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal(currencies.Count, response.Data.Count);
        
        for (int i = 0; i < currencies.Count; i++)
        {
            Assert.Equal(currencies[i].Name, response.Data[i].Name);
            Assert.Equal(currencies[i].Rate, response.Data[i].Rate);
        }
    }

    [Fact]
    public async Task GetAllCurrencies_Integration_NoCurrencies_ReturnsEmptyList()
    {
        // Act
        var result = await _controller.GetAllCurrencies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<List<CurrencyResponse>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("All currencies retrieved successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.Empty(response.Data);
    }

    #endregion

    #region AddToFavorites Integration Tests

    [Fact]
    public async Task AddToFavorites_Integration_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        var currencies = await CreateTestCurrenciesInDatabase(1);
        var currencyCode = currencies[0].Name;
        
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = currencyCode };

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Currency added to favorites successfully", response.Message);
        Assert.True(response.Data);
        
        var addedFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencies[0].Id);
        Assert.NotNull(addedFavorite);
    }

    [Fact]
    public async Task AddToFavorites_Integration_CurrencyAlreadyInFavorites_ReturnsFailure()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        var currencies = await CreateTestCurrenciesInDatabase(1);
        var currencyCode = currencies[0].Name;
        var existingFavorite = TestDataBuilder.CreateTestUserFavorite(userId, currencies[0].Id);
        
        await _context.UserFavorites.AddAsync(existingFavorite);
        await _context.SaveChangesAsync();
        
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = currencyCode };

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Currency already in favorites", response.Message);
        Assert.False(response.Data);
    }

    [Fact]
    public async Task AddToFavorites_Integration_CurrencyNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = "INVALID" };

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Currency with code INVALID not found", response.Message);
    }

    #endregion

    #region RemoveFromFavorites Integration Tests

    [Fact]
    public async Task RemoveFromFavorites_Integration_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        var currencies = await CreateTestCurrenciesInDatabase(1);
        var currencyCode = currencies[0].Name;
        var existingFavorite = TestDataBuilder.CreateTestUserFavorite(userId, currencies[0].Id);
        
        await _context.UserFavorites.AddAsync(existingFavorite);
        await _context.SaveChangesAsync();
        
        SetupUserClaims(userId);

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Currency removed from favorites successfully", response.Message);
        Assert.True(response.Data);
        
        var removedFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencies[0].Id);
        Assert.Null(removedFavorite);
    }

    [Fact]
    public async Task RemoveFromFavorites_Integration_CurrencyNotInFavorites_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        var currencies = await CreateTestCurrenciesInDatabase(1);
        var currencyCode = currencies[0].Name;
        
        SetupUserClaims(userId);

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Currency is not in user favorites", response.Message);
        Assert.False(response.Data);
    }

    [Fact]
    public async Task RemoveFromFavorites_Integration_CurrencyNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var user = await CreateTestUserInDatabase(userId);
        SetupUserClaims(userId);

        // Act
        var result = await _controller.RemoveFromFavorites("INVALID");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Currency with code INVALID not found", response.Message);
    }

    #endregion

    #region End-to-End Workflow Tests

    [Fact]
    public async Task EndToEnd_AddAndRemoveFavorites_CompleteWorkflow()
    {
        // Arrange
        var userId = 1;
        await CreateTestUserInDatabase(userId);
        var currencies = await CreateTestCurrenciesInDatabase(2);
        SetupUserClaims(userId);

        Assert.NotNull(currencies);
        Assert.True(currencies.Count >= 2, "Need at least 2 currencies for this test");

        var initialFavorites = await _controller.GetUserFavorites();
        var initialOkResult = Assert.IsType<OkObjectResult>(initialFavorites.Result);
        var initialResponse = Assert.IsType<ApiResponse<UserFavoritesResponse>>(initialOkResult.Value);
        Assert.NotNull(initialResponse.Data);
        Assert.Empty(initialResponse.Data.FavoriteCurrencies);

        var addRequest1 = new AddFavoriteRequest { CurrencyCode = currencies[0].Name };
        var addResult1 = await _controller.AddToFavorites(addRequest1);
        var addOkResult1 = Assert.IsType<OkObjectResult>(addResult1.Result);
        var addResponse1 = Assert.IsType<ApiResponse<bool>>(addOkResult1.Value);
        Assert.True(addResponse1.Data);

        var addRequest2 = new AddFavoriteRequest { CurrencyCode = currencies[1].Name };
        var addResult2 = await _controller.AddToFavorites(addRequest2);
        var addOkResult2 = Assert.IsType<OkObjectResult>(addResult2.Result);
        var addResponse2 = Assert.IsType<ApiResponse<bool>>(addOkResult2.Value);
        Assert.True(addResponse2.Data);

        var favoritesAfterAdd = await _controller.GetUserFavorites();
        var favoritesOkResult = Assert.IsType<OkObjectResult>(favoritesAfterAdd.Result);
        var favoritesResponse = Assert.IsType<ApiResponse<UserFavoritesResponse>>(favoritesOkResult.Value);
        Assert.NotNull(favoritesResponse.Data);
        Assert.Equal(2, favoritesResponse.Data.FavoriteCurrencies.Count);

        var removeResult = await _controller.RemoveFromFavorites(currencies[0].Name);
        var removeOkResult = Assert.IsType<OkObjectResult>(removeResult.Result);
        var removeResponse = Assert.IsType<ApiResponse<bool>>(removeOkResult.Value);
        Assert.True(removeResponse.Data);

        var favoritesAfterRemove = await _controller.GetUserFavorites();
        var finalOkResult = Assert.IsType<OkObjectResult>(favoritesAfterRemove.Result);
        var finalResponse = Assert.IsType<ApiResponse<UserFavoritesResponse>>(finalOkResult.Value);
        Assert.NotNull(finalResponse.Data);
        Assert.NotNull(finalResponse.Data.FavoriteCurrencies);
        Assert.Single(finalResponse.Data.FavoriteCurrencies);
        Assert.Equal(currencies[1].Name, finalResponse.Data.FavoriteCurrencies[0].CurrencyName);
    }

    #endregion
} 