using Shared.DTOs.DTOs;
using Shared.Identity.Services;
using Shared.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Domain.Services;

namespace UserService.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ISessionService sessionService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ISessionService _sessionService = sessionService;

    public async Task<ApiResponse<UserLoginResponse>> RegisterAsync(UserRegistrationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Password))
            {
                return CreateErrorResponse<UserLoginResponse>("Name and password are required", ["Name and password cannot be empty"]);
            }

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
            var sessionToken = _sessionService.CreateSession(createdUser.Id, createdUser.Name);

            return CreateSuccessResponse("User registered successfully", new UserLoginResponse
            {
                UserId = createdUser.Id,
                Name = createdUser.Name,
                SessionToken = sessionToken
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse<UserLoginResponse>("Registration failed", [ex.Message]);
        }
    }

    public async Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Password))
            {
                return CreateErrorResponse<UserLoginResponse>("Name and password are required", ["Name and password cannot be empty"]);
            }

            var user = await _userRepository.GetByNameAsync(request.Name);
            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                return CreateErrorResponse<UserLoginResponse>("Invalid credentials", ["Invalid username or password"]);
            }

            if (_sessionService.HasActiveSession(user.Id))
            {
                return CreateErrorResponse<UserLoginResponse>("User already has an active session", 
                    ["Please logout from your current session before logging in again"]);
            }

            var sessionToken = _sessionService.CreateSession(user.Id, user.Name);

            return CreateSuccessResponse("Login successful", new UserLoginResponse
            {
                UserId = user.Id,
                Name = user.Name,
                SessionToken = sessionToken
            });
        }
        catch (Exception ex)
        {
            return CreateErrorResponse<UserLoginResponse>("Login failed", [ex.Message]);
        }
    }

    public Task<ApiResponse<bool>> LogoutAsync(UserLogoutRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SessionToken))
            {
                return Task.FromResult(CreateErrorResponse<bool>("Session token is required", ["Session token cannot be empty"]));
            }

            if (!_sessionService.ValidateSession(request.SessionToken))
            {
                return Task.FromResult(CreateErrorResponse<bool>("Invalid session token", 
                    ["The provided session token is invalid or has expired"]));
            }

            _sessionService.InvalidateSession(request.SessionToken);

            return Task.FromResult(CreateSuccessResponse("Logout successful", true));
        }
        catch (Exception ex)
        {
            return Task.FromResult(CreateErrorResponse<bool>("Logout failed", [ex.Message]));
        }
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