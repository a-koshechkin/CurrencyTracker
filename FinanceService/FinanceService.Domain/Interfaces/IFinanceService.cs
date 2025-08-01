using Shared.DTOs.DTOs;

namespace FinanceService.Domain.Interfaces;

public interface IFinanceService
{
    Task<UserFavoritesResponse> GetUserFavoritesAsync(int userId);
} 