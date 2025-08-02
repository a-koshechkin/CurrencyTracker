using FinanceService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace FinanceService.Infrastructure.Repositories;

public class UserFavoriteRepository(FinanceDbContext context) : IUserFavoriteRepository
{
    private readonly FinanceDbContext _context = context;

    public async Task<IEnumerable<UserFavorite>> GetUserFavoritesAsync(int userId)
    {
        return await _context.UserFavorites
            .Include(uf => uf.User)
            .Include(uf => uf.Currency)
            .Where(uf => uf.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserFavorite?> GetUserFavoriteAsync(int userId, int currencyId)
    {
        return await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencyId);
    }

    public async Task AddUserFavoriteAsync(int userId, int currencyId)
    {
        var userFavorite = new UserFavorite
        {
            UserId = userId,
            CurrencyId = currencyId
        };

        _context.UserFavorites.Add(userFavorite);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserFavoriteAsync(int userId, int currencyId)
    {
        var userFavorite = await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.CurrencyId == currencyId);

        if (userFavorite != null)
        {
            _context.UserFavorites.Remove(userFavorite);
            await _context.SaveChangesAsync();
        }
    }
} 