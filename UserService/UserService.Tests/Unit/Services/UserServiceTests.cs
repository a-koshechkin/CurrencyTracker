using Moq;
using Shared.Domain.Entities;
using Shared.DTOs.DTOs;
using Shared.Identity.Services;
using System.Security.Claims;
using UserService.Domain.Interfaces;
using UserService.Tests.TestHelpers;

namespace UserService.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Application.Services.UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtService = new Mock<IJwtService>();
        _userService = new Application.Services.UserService(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtService.Object);
    }

    #region Registration Tests

    [Fact]
    public async Task RegisterAsync_ValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };
        var hashedPassword = "hashed_password_123";
        var createdUser = new User { Id = 1, Name = "testuser", Password = hashedPassword };
        var expectedToken = "jwt_token_123";

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync((User)null!);
        _mockPasswordHasher.Setup(x => x.HashPassword(request.Password))
            .Returns(hashedPassword);
        _mockUserRepository.Setup(x => x.CreateAsync(It.Is<User>(u => u.Name == request.Name)))
            .ReturnsAsync(createdUser);
        _mockJwtService.Setup(x => x.GenerateToken(createdUser.Id, createdUser.Name, "User"))
            .Returns(expectedToken);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("User registered successfully", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(createdUser.Id, result.Data.UserId);
        Assert.Equal(expectedToken, result.Data.AccessToken);
    }

    [Fact]
    public async Task RegisterAsync_UserAlreadyExists_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "existinguser", Password = "password123" };
        var existingUser = new User { Id = 1, Name = "existinguser", Password = "hashed_password" };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User already exists", result.Message);
        Assert.Contains("A user with this name already exists", result.Errors);
    }

    [Theory]
    [MemberData(nameof(TestDataBuilder.GetEmptyOrNullInputData), MemberType = typeof(TestDataBuilder))]
    public async Task RegisterAsync_EmptyOrNullInput_ReturnsErrorResponse(string name, string password)
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = name, Password = password };

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Name and password are required", result.Message);
        Assert.Contains("Name and password cannot be empty", result.Errors);
    }

    [Theory]
    [MemberData(nameof(TestDataBuilder.GetControlCharactersInputData), MemberType = typeof(TestDataBuilder))]
    public async Task RegisterAsync_ControlCharactersInName_ReturnsErrorResponse(string name, string password)
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = name, Password = password };

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Name and password are required", result.Message);
        Assert.Contains("Name contains invalid characters", result.Errors);
    }

    [Theory]
    [MemberData(nameof(TestDataBuilder.GetValidLengthInputData), MemberType = typeof(TestDataBuilder))]
    public async Task RegisterAsync_ValidLengthInput_ProceedsWithRegistration(string name, string password)
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = name, Password = password };
        var hashedPassword = "hashed_password_123";
        var createdUser = new User { Id = 1, Name = name, Password = hashedPassword };
        var expectedToken = "jwt_token_123";

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync((User)null!);
        _mockPasswordHasher.Setup(x => x.HashPassword(request.Password))
            .Returns(hashedPassword);
        _mockUserRepository.Setup(x => x.CreateAsync(It.Is<User>(u => u.Name == request.Name)))
            .ReturnsAsync(createdUser);
        _mockJwtService.Setup(x => x.GenerateToken(createdUser.Id, createdUser.Name, "User"))
            .Returns(expectedToken);

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("User registered successfully", result.Message);
    }

    [Theory]
    [MemberData(nameof(TestDataBuilder.GetTooLongInputData), MemberType = typeof(TestDataBuilder))]
    public async Task RegisterAsync_InputTooLong_ReturnsErrorResponse(string name, string password)
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = name, Password = password };

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Name and password are required", result.Message);
        Assert.Contains("Name or password exceeds maximum length", result.Errors);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "testuser", Password = "password123" };
        var existingUser = new User { Id = 1, Name = "testuser", Password = "hashed_password" };
        var expectedToken = "jwt_token_123";

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync(existingUser);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, existingUser.Password))
            .Returns(true);
        _mockJwtService.Setup(x => x.GenerateToken(existingUser.Id, existingUser.Name, "User"))
            .Returns(expectedToken);

        // Act
        var result = await _userService.LoginAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login successful", result.Message);
        Assert.NotNull(result.Data);
        Assert.Equal(existingUser.Id, result.Data.UserId);
        Assert.Equal(expectedToken, result.Data.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "nonexistentuser", Password = "password123" };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _userService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials", result.Message);
        Assert.Contains("Invalid username or password", result.Errors);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "testuser", Password = "wrongpassword" };
        var existingUser = new User { Id = 1, Name = "testuser", Password = "hashed_password" };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ReturnsAsync(existingUser);
        _mockPasswordHasher.Setup(x => x.VerifyPassword(request.Password, existingUser.Password))
            .Returns(false);

        // Act
        var result = await _userService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid credentials", result.Message);
        Assert.Contains("Invalid username or password", result.Errors);
    }

    [Theory]
    [MemberData(nameof(TestDataBuilder.GetEmptyOrNullInputData), MemberType = typeof(TestDataBuilder))]
    public async Task LoginAsync_EmptyOrNullInput_ReturnsErrorResponse(string name, string password)
    {
        // Arrange
        var request = new UserLoginRequest { Name = name, Password = password };

        // Act
        var result = await _userService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Name and password are required", result.Message);
        Assert.Contains("Name and password cannot be empty", result.Errors);
    }

    #endregion

    #region Logout Tests

    [Fact]
    public async Task LogoutAsync_ValidToken_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new UserLogoutRequest { AccessToken = "valid-token" };

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testuser")
        }));

        _mockJwtService.Setup(x => x.ValidateToken(request.AccessToken))
            .Returns(claimsPrincipal);

        // Act
        var result = await _userService.LogoutAsync(request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Logout successful", result.Message);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task LogoutAsync_InvalidToken_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserLogoutRequest { AccessToken = "invalid-token" };

        _mockJwtService.Setup(x => x.ValidateToken(request.AccessToken))
            .Returns((ClaimsPrincipal?)null);

        // Act
        var result = await _userService.LogoutAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Invalid token", result.Message);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task RegisterAsync_RepositoryThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserRegistrationRequest { Name = "testuser", Password = "password123" };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _userService.RegisterAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Registration failed", result.Message);
        Assert.Contains("Database connection failed", result.Errors);
    }

    [Fact]
    public async Task LoginAsync_RepositoryThrowsException_ReturnsErrorResponse()
    {
        // Arrange
        var request = new UserLoginRequest { Name = "testuser", Password = "password123" };

        _mockUserRepository.Setup(x => x.GetByNameAsync(request.Name))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _userService.LoginAsync(request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Login failed", result.Message);
        Assert.Contains("Database connection failed", result.Errors);
    }

    #endregion
}