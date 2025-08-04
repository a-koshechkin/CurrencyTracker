using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.DTOs.DTOs;
using Shared.Identity.Services;
using Shared.Domain.Entities;
using UserService.Api.Controllers;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.Repositories;

namespace UserService.Tests.Integration;

public class UserServiceIntegrationTests : IDisposable
{
    private readonly UserDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly UserController _controller;

    public UserServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);
        _userRepository = new UserRepository(_context);
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtService = new Mock<IJwtService>();
        
        var _userService = new Application.Services.UserService(
            _userRepository,
            _mockPasswordHasher.Object,
            _mockJwtService.Object);

        var mockLogger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userService, mockLogger.Object);
    }

    #region End-to-End Registration Flow Tests

    [Fact]
    public async Task Register_CompleteFlow_UserCreatedSuccessfully()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "newuser", Password = "password123" };
        var hashedPassword = "hashed_password_123";
        var expectedToken = "jwt_token_123";

        _mockPasswordHasher.Setup(x => x.HashPassword(request.Password))
            .Returns(hashedPassword);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<int>(), request.Name, "User"))
            .Returns(expectedToken);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User registered successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal("newuser", response.Data.Name);
        Assert.Equal(expectedToken, response.Data.AccessToken);

        var savedUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == "newuser");
        Assert.NotNull(savedUser);
        Assert.Equal(hashedPassword, savedUser.Password);
    }

    [Fact]
    public async Task Register_DuplicateUser_ReturnsError()
    {
        // Arrange
        var existingUser = new User 
        { 
            Name = "existinguser", 
            Password = "hashed_password" 
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new UserRegistrationRequest { Name = "existinguser", Password = "password123" };

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Equal("User already exists", response.Message);
    }

    #endregion

    #region End-to-End Login Flow Tests

    [Fact]
    public async Task Login_CompleteFlow_UserAuthenticatedSuccessfully()
    {
        // Arrange
        var existingUser = new User 
        { 
            Name = "testuser", 
            Password = "hashed_password" 
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new UserLoginRequest { Name = "testuser", Password = "password123" };
        var expectedToken = "jwt_token_123";

        _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, existingUser.Password))
            .Returns(true);
        _mockJwtService.Setup(x => x.GenerateToken(existingUser.Id, existingUser.Name, "User"))
            .Returns(expectedToken);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal("User logged in successfully", response.Message);
        Assert.NotNull(response.Data);
        Assert.Equal("testuser", response.Data.Name);
        Assert.Equal(expectedToken, response.Data.AccessToken);
    }

    [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "nonexistentuser", Password = "password123" };

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Invalid credentials", response.Message);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var existingUser = new User 
        { 
            Name = "testuser", 
            Password = "hashed_password" 
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new UserLoginRequest { Name = "testuser", Password = "wrongpassword" };

        _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, existingUser.Password))
            .Returns(false);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Equal("Invalid credentials", response.Message);
    }

    #endregion

    #region Authentication Flow Tests

    [Fact]
    public async Task RegisterThenLogin_CompleteAuthenticationFlow_Success()
    {
        // Arrange
        var registrationRequest = new UserRegistrationRequest { Name = "authuser", Password = "password123" };
        var loginRequest = new UserLoginRequest { Name = "authuser", Password = "password123" };
        var hashedPassword = "hashed_password_123";
        var registrationToken = "registration_token_123";
        var loginToken = "login_token_123";

        _mockPasswordHasher.Setup(x => x.HashPassword(registrationRequest.Password))
            .Returns(hashedPassword);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(loginRequest.Password, hashedPassword))
            .Returns(true);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<int>(), registrationRequest.Name, "User"))
            .Returns(registrationToken);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<int>(), loginRequest.Name, "User"))
            .Returns(loginToken);

        // Act
        var registrationResult = await _controller.Register(registrationRequest);

        // Assert
        var registrationOkResult = Assert.IsType<OkObjectResult>(registrationResult.Result);
        var registrationResponse = Assert.IsType<ApiResponse<UserLoginResponse>>(registrationOkResult.Value);
        Assert.True(registrationResponse.Success);

        // Act
        var loginResult = await _controller.Login(loginRequest);

        // Assert
        var loginOkResult = Assert.IsType<OkObjectResult>(loginResult.Result);
        var loginResponse = Assert.IsType<ApiResponse<UserLoginResponse>>(loginOkResult.Value);
        Assert.True(loginResponse.Success);
        Assert.Equal("authuser", loginResponse.Data.Name);
    }

    #endregion

    #region Error Handling Integration Tests

    [Fact]
    public async Task Register_WhenPasswordHasherFails_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };
        
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Throws(new InvalidOperationException("Password hashing service unavailable"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Registration failed", response.Message);
    }

    [Fact]
    public async Task Register_WhenJwtServiceFails_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };
        var hashedPassword = "hashed_password_123";
        
        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns(hashedPassword);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), "User"))
            .Throws(new InvalidOperationException("JWT service unavailable"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(badRequestResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Registration failed", response.Message);
    }

    [Fact]
    public async Task Login_WhenPasswordVerificationFails_ReturnsErrorResponse()
    {
        // Arrange
        var existingUser = new User 
        { 
            Name = "testuser", 
            Password = "hashed_password" 
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new UserLoginRequest { Name = "testuser", Password = "wrongpassword" };
        
        _mockPasswordHasher.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new InvalidOperationException("Password verification service unavailable"));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<UserLoginResponse>>(unauthorizedResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Login failed", response.Message);
    }

    #endregion

    #region Database Integration Tests

    [Fact]
    public async Task Database_ConcurrentUserCreation_HandlesCorrectly()
    {
        // Arrange
        var request1 = new UserRegistrationRequest { Name = "concurrentuser", Password = "password123" };
        var request2 = new UserRegistrationRequest { Name = "concurrentuser", Password = "password456" };
        var hashedPassword = "hashed_password_123";
        var expectedToken = "jwt_token_123";

        _mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns(hashedPassword);
        _mockJwtService.Setup(x => x.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), "User"))
            .Returns(expectedToken);

        // Act
        var task1 = _controller.Register(request1);
        var task2 = _controller.Register(request2);

        var results = await Task.WhenAll(task1, task2);

        // Assert
        var successCount = results.Count(r => 
            r.Result is OkObjectResult okResult && 
            okResult.Value is ApiResponse<UserLoginResponse> response && 
            response.Success);
        var failureCount = results.Count(r => 
            r.Result is BadRequestObjectResult badRequestResult && 
            badRequestResult.Value is ApiResponse<UserLoginResponse> response && 
            !response.Success);

        Assert.Equal(1, successCount);
        Assert.Equal(1, failureCount);
    }

    [Fact]
    public async Task Database_UserRetrieval_WorksCorrectly()
    {
        // Arrange
        var users = new[]
        {
            new User { Name = "user1", Password = "hash1" },
            new User { Name = "user2", Password = "hash2" },
            new User { Name = "user3", Password = "hash3" }
        };

        foreach (var user in users)
        {
            _context.Users.Add(user);
        }
        await _context.SaveChangesAsync();

        // Act & Assert
        foreach (var user in users)
        {
            var retrievedUser = await _userRepository.GetByNameAsync(user.Name);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.Name, retrievedUser.Name);
            Assert.Equal(user.Password, retrievedUser.Password);
        }
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
} 