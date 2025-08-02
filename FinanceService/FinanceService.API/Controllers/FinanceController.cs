using FinanceService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;

namespace FinanceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController(IFinanceService financeService) : ControllerBase
{
    private readonly IFinanceService _financeService = financeService;

    [HttpGet("favorites")]
    public async Task<ActionResult<ApiResponse<UserFavoritesResponse>>> GetUserFavorites()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<UserFavoritesResponse>
                {
                    Success = false,
                    Message = "User ID not found",
                    Errors = ["User ID not found"]
                });
            }

            var result = await _financeService.GetUserFavoritesAsync(userId);
            return Ok(new ApiResponse<UserFavoritesResponse>
            {
                Success = true,
                Message = "User favorites retrieved successfully",
                Data = result
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ApiResponse<UserFavoritesResponse>
            {
                Success = false,
                Message = ex.Message,
                Errors = [ex.Message]
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<UserFavoritesResponse>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                Errors = ["Internal server error"]
            });
        }
    }

    [HttpGet("currencies")]
    public async Task<ActionResult<ApiResponse<List<CurrencyResponse>>>> GetAllCurrencies()
    {
        try
        {
            var result = await _financeService.GetAllCurrenciesAsync();
            return Ok(new ApiResponse<List<CurrencyResponse>>
            {
                Success = true,
                Message = "All currencies retrieved successfully",
                Data = result
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<List<CurrencyResponse>>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                Errors = ["Internal server error"]
            });
        }
    }

    [HttpPost("favorites")]
    public async Task<ActionResult<ApiResponse<bool>>> AddToFavorites([FromBody] AddFavoriteRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User ID not found",
                    Errors = ["User ID not found"]
                });
            }

            if (string.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid currency code",
                    Errors = ["Currency code cannot be empty"]
                });
            }

            var result = await _financeService.AddToFavoritesAsync(userId, request.CurrencyCode);
            
            if (result)
            {
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Currency added to favorites successfully",
                    Data = result
                });
            }
            else
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Currency already in favorites",
                    Errors = ["Currency is already in user favorites"]
                });
            }
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message,
                Errors = [ex.Message]
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                Errors = ["Internal server error"]
            });
        }
    }

    [HttpDelete("favorites/{currencyCode}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveFromFavorites(string currencyCode)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "User ID not found",
                    Errors = ["User ID not found"]
                });
            }

            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Invalid currency code",
                    Errors = ["Currency code cannot be empty"]
                });
            }

            var result = await _financeService.RemoveFromFavoritesAsync(userId, currencyCode);
            
            if (result)
            {
                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Currency removed from favorites successfully",
                    Data = result
                });
            }
            else
            {
                return NotFound(new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Currency is not in user favorites",
                    Errors = ["Currency is not in user favorites"]
                });
            }
        }
        catch (ArgumentException ex)
        {
            return NotFound(new ApiResponse<bool>
            {
                Success = false,
                Message = ex.Message,
                Errors = [ex.Message]
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ApiResponse<bool>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                Errors = ["Internal server error"]
            });
        }
    }
} 