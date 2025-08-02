using Shared.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface IUserFavoriteRepository
{
    Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId);
    Task<UserFavorite?> GetUserFavoriteAsync(int userId, int currencyId);
    Task AddUserFavoriteAsync(int userId, int currencyId);
    Task RemoveUserFavoriteAsync(int userId, int currencyId);
} 