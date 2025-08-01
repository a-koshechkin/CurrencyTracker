using FinanceService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.DTOs;
using Shared.Infrastructure.Controllers;

namespace FinanceService.API.Controllers;

public class FinanceController(IFinanceService financeService) : BaseApiController
{
    private readonly IFinanceService _financeService = financeService;

    [HttpGet("favorites/{userId}")]
    public async Task<ActionResult<ApiResponse<UserFavoritesResponse>>> GetUserFavorites(int userId)
    {
        try
        {
            if (userId <= 0)
            {
                return BadRequest<UserFavoritesResponse>("Invalid user ID", ["User ID must be greater than 0"]);
            }

            var result = await _financeService.GetUserFavoritesAsync(userId);
            return Success(result, "User favorites retrieved successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound<UserFavoritesResponse>(ex.Message);
        }
        catch (Exception)
        {
            return InternalServerError<UserFavoritesResponse>();
        }
    }
} 