using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;

namespace Shared.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> Success<T>(T data, string message = "Operation completed successfully")
    {
        return Ok(new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    protected ActionResult<ApiResponse<T>> BadRequest<T>(string message, List<string> errors)
    {
        return BadRequest(new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        });
    }

    protected ActionResult<ApiResponse<T>> NotFound<T>(string message)
    {
        return NotFound(new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = [message]
        });
    }

    protected ActionResult<ApiResponse<T>> InternalServerError<T>(string message = "An error occurred while processing your request")
    {
        return StatusCode(500, new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = ["Internal server error"]
        });
    }

    protected ActionResult<ApiResponse<T>> UnauthorizedResponse<T>(string message)
    {
        return StatusCode(401, new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = [message]
        });
    }
} 