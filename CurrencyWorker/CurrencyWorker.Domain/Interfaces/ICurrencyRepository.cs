using Shared.Domain.Entities;

namespace CurrencyWorker.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task UpdateCurrencyRatesAsync(IEnumerable<CurrencyRate> rates, CancellationToken cancellationToken = default);
} 