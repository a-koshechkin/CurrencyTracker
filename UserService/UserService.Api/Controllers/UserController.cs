using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
using Shared.Infrastructure.Controllers;
using UserService.Domain.Services;

namespace UserService.Api.Controllers;

public class UserController(IUserService userService) : BaseApiController
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Register([FromBody] UserRegistrationRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        
        if (!result.Success)
            return BadRequest<UserLoginResponse>(result.Message, result.Errors);
            
        return Success(result.Data!, "User registered successfully");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Login([FromBody] UserLoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        
        if (!result.Success)
            return UnauthorizedResponse<UserLoginResponse>(result.Message);
            
        return Success(result.Data!, "User logged in successfully");
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] UserLogoutRequest request)
    {
        var result = await _userService.LogoutAsync(request);
        
        if (!result.Success)
        {
            if (result.Message.Contains("Invalid"))
                return UnauthorizedResponse<bool>(result.Message);
            else
                return BadRequest<bool>(result.Message, result.Errors);
        }
            
        return Success(result.Data!, "User logged out successfully");
    }
} 