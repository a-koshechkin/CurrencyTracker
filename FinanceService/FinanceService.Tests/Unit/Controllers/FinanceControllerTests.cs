using FinanceService.API.Controllers;
using FinanceService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs.DTOs;
using System.Security.Claims;

namespace FinanceService.Tests.Unit.Controllers;

public class FinanceControllerTests
{
    private readonly Mock<IFinanceService> _mockFinanceService;
    private readonly FinanceController _controller;
    private readonly HttpContext _httpContext;

    public FinanceControllerTests()
    {
        _mockFinanceService = new Mock<IFinanceService>();
        _controller = new FinanceController(_mockFinanceService.Object);
        
        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
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

    #region GetUserFavorites Tests

    [Fact]
    public async Task GetUserFavorites_ValidUserId_ReturnsOkWithFavorites()
    {
        // Arrange
        var userId = 1;
        SetupUserClaims(userId);
        var expectedResponse = new UserFavoritesResponse
        {
            FavoriteCurrencies = new List<FavoriteCurrencyInfo>
            {
                new() { CurrencyName = "USD", Rate = 1.0m },
                new() { CurrencyName = "EUR", Rate = 0.85m }
            }
        };

        _mockFinanceService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User favorites retrieved successfully", response.Message);
        Assert.Equal(expectedResponse, response.Data);
    }

    [Fact]
    public async Task GetUserFavorites_NoUserClaim_ReturnsUnauthorized()
    {
        // Arrange
        _httpContext.User = new ClaimsPrincipal();

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User ID not found", response.Message);
    }

    [Fact]
    public async Task GetUserFavorites_InvalidUserIdClaim_ReturnsUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "invalid-id")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _httpContext.User = principal;

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User ID not found", response.Message);
    }

    [Fact]
    public async Task GetUserFavorites_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        SetupUserClaims(userId);

        _mockFinanceService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ThrowsAsync(new ArgumentException($"User with ID {userId} not found"));

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal($"User with ID {userId} not found", response.Message);
    }

    [Fact]
    public async Task GetUserFavorites_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var userId = 1;
        SetupUserClaims(userId);

        _mockFinanceService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(statusCodeResult.Value);
        Assert.False(response.Success);
        Assert.Equal("An error occurred while processing your request", response.Message);
    }

    #endregion

    #region GetAllCurrencies Tests

    [Fact]
    public async Task GetAllCurrencies_ValidRequest_ReturnsOkWithCurrencies()
    {
        // Arrange
        var expectedCurrencies = new List<CurrencyResponse>
        {
            new() { Name = "USD", Rate = 1.0m },
            new() { Name = "EUR", Rate = 0.85m },
            new() { Name = "GBP", Rate = 0.73m }
        };

        _mockFinanceService.Setup(x => x.GetAllCurrenciesAsync())
            .ReturnsAsync(expectedCurrencies);

        // Act
        var result = await _controller.GetAllCurrencies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<List<CurrencyResponse>>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("All currencies retrieved successfully", response.Message);
        Assert.Equal(expectedCurrencies, response.Data);
    }

    [Fact]
    public async Task GetAllCurrencies_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockFinanceService.Setup(x => x.GetAllCurrenciesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllCurrencies();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var response = Assert.IsType<ApiResponse<List<CurrencyResponse>>>(statusCodeResult.Value);
        Assert.False(response.Success);
        Assert.Equal("An error occurred while processing your request", response.Message);
    }

    #endregion

    #region AddToFavorites Tests

    [Fact]
    public async Task AddToFavorites_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = currencyCode };

        _mockFinanceService.Setup(x => x.AddToFavoritesAsync(userId, currencyCode))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Currency added to favorites successfully", response.Message);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task AddToFavorites_NoUserClaim_ReturnsUnauthorized()
    {
        // Arrange
        _httpContext.User = new ClaimsPrincipal();
        var request = new AddFavoriteRequest { CurrencyCode = "USD" };

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User ID not found", response.Message);
    }

    [Fact]
    public async Task AddToFavorites_CurrencyAlreadyInFavorites_ReturnsBadRequest()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = currencyCode };

        _mockFinanceService.Setup(x => x.AddToFavoritesAsync(userId, currencyCode))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Currency already in favorites", response.Message);
    }

    [Fact]
    public async Task AddToFavorites_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        var currencyCode = "USD";
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = currencyCode };

        _mockFinanceService.Setup(x => x.AddToFavoritesAsync(userId, currencyCode))
            .ThrowsAsync(new ArgumentException($"User with ID {userId} not found"));

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal($"User with ID {userId} not found", response.Message);
    }

    [Fact]
    public async Task AddToFavorites_CurrencyNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "INVALID";
        SetupUserClaims(userId);
        var request = new AddFavoriteRequest { CurrencyCode = currencyCode };

        _mockFinanceService.Setup(x => x.AddToFavoritesAsync(userId, currencyCode))
            .ThrowsAsync(new ArgumentException($"Currency with code {currencyCode} not found"));

        // Act
        var result = await _controller.AddToFavorites(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal($"Currency with code {currencyCode} not found", response.Message);
    }

    #endregion

    #region RemoveFromFavorites Tests

    [Fact]
    public async Task RemoveFromFavorites_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        SetupUserClaims(userId);

        _mockFinanceService.Setup(x => x.RemoveFromFavoritesAsync(userId, currencyCode))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("Currency removed from favorites successfully", response.Message);
        Assert.True(response.Data);
    }

    [Fact]
    public async Task RemoveFromFavorites_NoUserClaim_ReturnsUnauthorized()
    {
        // Arrange
        _httpContext.User = new ClaimsPrincipal();
        var currencyCode = "USD";

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User ID not found", response.Message);
    }

    [Fact]
    public async Task RemoveFromFavorites_CurrencyNotInFavorites_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "USD";
        SetupUserClaims(userId);

        _mockFinanceService.Setup(x => x.RemoveFromFavoritesAsync(userId, currencyCode))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Currency is not in user favorites", response.Message);
    }

    [Fact]
    public async Task RemoveFromFavorites_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 999;
        var currencyCode = "USD";
        SetupUserClaims(userId);

        _mockFinanceService.Setup(x => x.RemoveFromFavoritesAsync(userId, currencyCode))
            .ThrowsAsync(new ArgumentException($"User with ID {userId} not found"));

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal($"User with ID {userId} not found", response.Message);
    }

    [Fact]
    public async Task RemoveFromFavorites_CurrencyNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        var currencyCode = "INVALID";
        SetupUserClaims(userId);

        _mockFinanceService.Setup(x => x.RemoveFromFavoritesAsync(userId, currencyCode))
            .ThrowsAsync(new ArgumentException($"Currency with code {currencyCode} not found"));

        // Act
        var result = await _controller.RemoveFromFavorites(currencyCode);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal($"Currency with code {currencyCode} not found", response.Message);
    }

    #endregion
} 