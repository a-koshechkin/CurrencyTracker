using Shared.Domain.Entities;
using Shared.DTOs.DTOs;
using Shared.Identity.Services;
using UserService.Domain.Interfaces;
using UserService.Domain.Services;

namespace UserService.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtService jwtService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtService _jwtService = jwtService;

    public async Task<ApiResponse<UserLoginResponse>> RegisterAsync(UserRegistrationRequest request)
    {
        if (request?.Name == null || string.IsNullOrWhiteSpace(request.Name.Trim()) || 
            request?.Password == null || string.IsNullOrWhiteSpace(request.Password.Trim()))
        {
            return CreateErrorResponse<UserLoginResponse>("Name and password are required", ["Name and password cannot be empty"]);
        }

        if (ContainsControlCharacters(request.Name))
        {
            return CreateErrorResponse<UserLoginResponse>("Name and password are required", ["Name contains invalid characters"]);
        }

        if (request.Name.Length > 100 || request.Password.Length > 255)
        {
            return CreateErrorResponse<UserLoginResponse>("Name and password are required", ["Name or password exceeds maximum length"]);
        }

        try
        {
            var existingUser = await _userRepository.GetByNameAsync(request.Name);
            if (existingUser != null)
            {
                return CreateErrorResponse<UserLoginResponse>("User already exists", ["A user with this name already exists"]);
            }

            var hashedPassword = _passwordHasher.HashPassword(request.Password);
            var user = new User
            {
                Name = request.Name,
                Password = hashedPassword
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var accessToken = _jwtService.GenerateToken(createdUser.Id, createdUser.Name);

            return CreateSuccessResponse("User registered successfully", new UserLoginResponse
            {
                Name = createdUser.Name,
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse<UserLoginResponse>("Registration failed", [ex.Message]);
        }
    }

    public async Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request)
    {
        if (request?.Name == null || string.IsNullOrWhiteSpace(request.Name.Trim()) || 
            request?.Password == null || string.IsNullOrWhiteSpace(request.Password.Trim()))
        {
            return CreateErrorResponse<UserLoginResponse>("Name and password are required", ["Name and password cannot be empty"]);
        }

        try
        {
            var user = await _userRepository.GetByNameAsync(request.Name);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return CreateErrorResponse<UserLoginResponse>("Invalid credentials", ["Invalid username or password"]);
            }

            var accessToken = _jwtService.GenerateToken(user.Id, user.Name);

            return CreateSuccessResponse("Login successful", new UserLoginResponse
            {
                Name = user.Name,
                AccessToken = accessToken,
                TokenType = "Bearer",
                ExpiresIn = 3600
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse<UserLoginResponse>("Login failed", [ex.Message]);
        }
    }

    public Task<ApiResponse<bool>> LogoutAsync(UserLogoutRequest request)
    {
        if (request?.AccessToken == null || string.IsNullOrWhiteSpace(request.AccessToken.Trim()))
        {
            return Task.FromResult(CreateErrorResponse<bool>("Access token is required", ["Access token cannot be empty"]));
        }

        try
        {
            var tokenValidation = _jwtService.ValidateToken(request.AccessToken);
            if (tokenValidation == null)
            {
                return Task.FromResult(CreateErrorResponse<bool>("Invalid token", ["Token validation failed"]));
            }

            return Task.FromResult(CreateSuccessResponse("Logout successful", true));
        }
        catch (Exception ex)
        {
            return Task.FromResult(CreateErrorResponse<bool>("Logout failed", [ex.Message]));
        }
    }

    private static bool ContainsControlCharacters(string input)
    {
        return input.Any(c => char.IsControl(c) && c != '\t' && c != '\n' && c != '\r' && c != '\f' && c != '\v');
    }

    private static ApiResponse<T> CreateSuccessResponse<T>(string message, T data)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    private static ApiResponse<T> CreateErrorResponse<T>(string message, List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
} 