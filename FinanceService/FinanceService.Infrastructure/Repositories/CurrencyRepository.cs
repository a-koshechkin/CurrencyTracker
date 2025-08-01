using FinanceService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;

namespace FinanceService.Infrastructure.Repositories;

public class CurrencyRepository(FinanceDbContext context) : ICurrencyRepository
{
    private readonly FinanceDbContext _context = context;

    public async Task<IEnumerable<Currency>> GetCurrenciesByIdsAsync(IEnumerable<int> currencyIds)
    {
        return await _context.Currencies
            .Where(c => currencyIds.Contains(c.Id))
            .ToListAsync();
    }
} 