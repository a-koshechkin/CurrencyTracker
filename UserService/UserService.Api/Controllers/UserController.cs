using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
using UserService.Domain.Services;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, ILogger<UserController> logger) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ILogger<UserController> _logger = logger;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Register([FromBody] UserRegistrationRequest request)
    {
        try
        {
            var result = await _userService.RegisterAsync(request);
            
            if (!result.Success)
            {
                _logger.LogWarning("User registration failed for user: {UserName}", request?.Name);
                return BadRequest(new ApiResponse<UserLoginResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
                
            _logger.LogInformation("User registered successfully: {UserName}", request?.Name);
            return Ok(new ApiResponse<UserLoginResponse>
            {
                Success = true,
                Message = "User registered successfully",
                Data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during registration for user: {UserName}", request?.Name);
            throw;
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Login([FromBody] UserLoginRequest request)
    {
        try
        {
            var result = await _userService.LoginAsync(request);
            
            if (!result.Success)
            {
                _logger.LogWarning("Login failed for user: {UserName}", request?.Name);
                return Unauthorized(new ApiResponse<UserLoginResponse>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = [result.Message]
                });
            }
                
            _logger.LogInformation("User logged in successfully: {UserName}", request?.Name);
            return Ok(new ApiResponse<UserLoginResponse>
            {
                Success = true,
                Message = "User logged in successfully",
                Data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during login for user: {UserName}", request?.Name);
            throw;
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] UserLogoutRequest request)
    {
        try
        {
            var result = await _userService.LogoutAsync(request);
            
            if (!result.Success)
            {
                if (result.Message.Contains("Invalid"))
                {
                    _logger.LogWarning("Logout unauthorized: {Message}", result.Message);
                    return Unauthorized(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = [result.Message]
                    });
                }
                else
                {
                    _logger.LogWarning("Logout failed: {Message}", result.Message);
                    return BadRequest(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }
            }
                
            _logger.LogInformation("User logged out successfully");
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "User logged out successfully",
                Data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred during logout");
            throw;
        }
    }
} 