using Shared.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface IUserFavoriteRepository
{
    Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId);
} 