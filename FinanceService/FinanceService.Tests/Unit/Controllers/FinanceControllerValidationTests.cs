using System.Security.Claims;
using FinanceService.API.Controllers;
using FinanceService.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.DTOs.DTOs;

namespace FinanceService.Tests.Unit.Controllers;

public class FinanceControllerValidationTests
{
    private readonly Mock<IFinanceService> _mockFinanceService;
    private readonly FinanceController _controller;
    private readonly HttpContext _httpContext;

    public FinanceControllerValidationTests()
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

    #region Authentication/Authorization Tests

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
    public async Task RemoveFromFavorites_NoUserClaim_ReturnsUnauthorized()
    {
        // Arrange
        _httpContext.User = new ClaimsPrincipal();

        // Act
        var result = await _controller.RemoveFromFavorites("USD");

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User ID not found", response.Message);
    }

    #endregion

    #region HTTP Error Handling Tests

    [Fact]
    public async Task GetUserFavorites_ServiceThrowsArgumentException_ReturnsNotFound()
    {
        // Arrange
        var userId = 1;
        SetupUserClaims(userId);
        _mockFinanceService.Setup(x => x.GetUserFavoritesAsync(userId))
            .ThrowsAsync(new ArgumentException("User not found"));

        // Act
        var result = await _controller.GetUserFavorites();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User not found", response.Message);
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
        var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
        var response = Assert.IsType<ApiResponse<UserFavoritesResponse>>(internalServerErrorResult.Value);
        Assert.False(response.Success);
        Assert.Equal("An error occurred while processing your request", response.Message);
    }

    #endregion
} 