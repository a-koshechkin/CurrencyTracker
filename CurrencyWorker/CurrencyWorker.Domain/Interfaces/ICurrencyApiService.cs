using Shared.Domain.Entities;

namespace CurrencyWorker.Domain.Interfaces;

public interface ICurrencyApiService
{
    Task<IEnumerable<CurrencyRate>> GetCurrentRatesAsync(CancellationToken cancellationToken = default);
} 