using Shared.Domain.Entities;

namespace FinanceService.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<IEnumerable<Currency>> GetCurrenciesByIdsAsync(IEnumerable<int> currencyIds);
    Task<List<Currency>> GetAllCurrenciesAsync();
    Task<Currency?> GetCurrencyByNameAsync(string code);
} 