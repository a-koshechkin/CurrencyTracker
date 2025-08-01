using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
using UserService.Domain.Services;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Register([FromBody] UserRegistrationRequest request)
    {
        var result = await _userService.RegisterAsync(request);
        
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<UserLoginResponse>>> Login([FromBody] UserLoginRequest request)
    {
        var result = await _userService.LoginAsync(request);
        
        if (!result.Success)
            return Unauthorized(result);
            
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] UserLogoutRequest request)
    {
        var result = await _userService.LogoutAsync(request);
        
        if (!result.Success)
        {
            if (result.Message.Contains("Invalid session token"))
                return Unauthorized(result);
            else
                return BadRequest(result);
        }
            
        return Ok(result);
    }
} 