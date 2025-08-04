using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DTOs.DTOs;
using UserService.Api.Controllers;
using UserService.Domain.Services;

namespace UserService.Tests.Unit.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        var mockLogger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_mockUserService.Object, mockLogger.Object);
    }

    #region Registration Tests

    [Fact]
    public async Task Register_ServiceReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };

        _mockUserService.Setup(x => x.RegisterAsync(It.IsAny<UserRegistrationRequest>()))
            .ReturnsAsync((ApiResponse<UserLoginResponse>)null!);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Service returned null response", response.Message);
    }

    [Fact]
    public async Task Register_ServiceThrowsException_ThrowsException()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };

        _mockUserService.Setup(x => x.RegisterAsync(It.IsAny<UserRegistrationRequest>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Register(request));
    }

    [Fact]
    public async Task Register_ServiceReturnsSuccess_ReturnsOkResult()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };
        var expectedResponse = new ApiResponse<UserLoginResponse>
        {
            Success = true,
            Message = "User registered successfully",
            Data = new UserLoginResponse
            {
                UserId = 1,
                Name = "testuser",
                AccessToken = "valid-token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            }
        };

        _mockUserService.Setup(x => x.RegisterAsync(It.IsAny<UserRegistrationRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User registered successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.UserId);
        Assert.Equal("testuser", response.Data.Name);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ServiceReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "testuser", Password = "password123" };

        _mockUserService.Setup(x => x.LoginAsync(It.IsAny<UserLoginRequest>()))
            .ReturnsAsync((ApiResponse<UserLoginResponse>)null!);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Service returned null response", response.Message);
    }



    [Fact]
    public async Task Login_ServiceThrowsException_ThrowsException()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "testuser", Password = "password123" };

        _mockUserService.Setup(x => x.LoginAsync(It.IsAny<UserLoginRequest>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Login(request));
    }

    [Fact]
    public async Task Login_ServiceReturnsSuccess_ReturnsOkResult()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "testuser", Password = "password123" };
        var expectedResponse = new ApiResponse<UserLoginResponse>
        {
            Success = true,
            Message = "Login successful",
            Data = new UserLoginResponse
            {
                UserId = 1,
                Name = "testuser",
                AccessToken = "valid-token",
                TokenType = "Bearer",
                ExpiresIn = 3600
            }
        };

        _mockUserService.Setup(x => x.LoginAsync(It.IsAny<UserLoginRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User logged in successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.UserId);
        Assert.Equal("testuser", response.Data.Name);
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task Logout_ServiceReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        var request = new UserLogoutRequest { AccessToken = "valid-token" };

        _mockUserService.Setup(x => x.LogoutAsync(It.IsAny<UserLogoutRequest>()))
            .ReturnsAsync((ApiResponse<bool>)null!);

        // Act
        var result = await _controller.Logout(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Service returned null response", response.Message);
    }

    [Fact]
    public async Task Logout_ServiceReturnsSuccess_ReturnsOkResult()
    {
        // Arrange
        var request = new UserLogoutRequest { AccessToken = "valid-token" };
        var expectedResponse = new ApiResponse<bool>
        {
            Success = true,
            Message = "Logout successful",
            Data = true
        };

        _mockUserService.Setup(x => x.LogoutAsync(It.IsAny<UserLogoutRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Logout(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<bool>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User logged out successfully", response.Message);
        Assert.True(response.Data);
    }

    #endregion

} 