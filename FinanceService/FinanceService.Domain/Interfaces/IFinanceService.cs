using Shared.DTOs.DTOs;

namespace FinanceService.Domain.Interfaces;

public interface IFinanceService
{
    Task<UserFavoritesResponse> GetUserFavoritesAsync(int userId);
    Task<List<CurrencyResponse>> GetAllCurrenciesAsync();
    Task<bool> AddToFavoritesAsync(int userId, string currencyCode);
    Task<bool> RemoveFromFavoritesAsync(int userId, string currencyCode);
} 