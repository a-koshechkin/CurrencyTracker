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


} 