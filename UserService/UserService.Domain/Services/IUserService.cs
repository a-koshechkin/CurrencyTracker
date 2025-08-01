using Shared.DTOs.DTOs;

namespace UserService.Domain.Services;

public interface IUserService
{
    Task<ApiResponse<UserLoginResponse>> RegisterAsync(UserRegistrationRequest request);
    Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request);
    Task<ApiResponse<bool>> LogoutAsync(UserLogoutRequest request);
} 