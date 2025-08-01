using FinanceService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace FinanceService.Infrastructure.Repositories;

public class UserRepository(FinanceDbContext context) : IUserRepository
{
    private readonly FinanceDbContext _context = context;

    public async Task<bool> UserExistsAsync(int userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }
} 