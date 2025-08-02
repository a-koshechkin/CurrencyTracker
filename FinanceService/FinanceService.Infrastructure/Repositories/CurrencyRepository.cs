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

    public async Task<List<Currency>> GetAllCurrenciesAsync()
    {
        return await _context.Currencies
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Currency?> GetCurrencyByNameAsync(string name)
    {
        return await _context.Currencies
            .FirstOrDefaultAsync(c => c.Name == name);
    }
} 