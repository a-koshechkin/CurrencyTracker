using CurrencyWorker.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CurrencyWorker.Application.Services;

public class CurrencyUpdateService(
    ICurrencyApiService currencyApiService,
    ICurrencyRepository currencyRepository,
    ILogger<CurrencyUpdateService> logger)
{
    private readonly ICurrencyApiService _currencyApiService = currencyApiService;
    private readonly ICurrencyRepository _currencyRepository = currencyRepository;
    private readonly ILogger<CurrencyUpdateService> _logger = logger;

    public async Task<bool> UpdateCurrencyRatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting currency rates update from Russian Central Bank...");

            var rates = await _currencyApiService.GetCurrentRatesAsync(cancellationToken);
            if (!rates.Any())
            {
                _logger.LogWarning("No rates received from API");
                return false;
            }

            _logger.LogInformation("Received {Count} currency rates from API", rates.Count());

            await _currencyRepository.UpdateCurrencyRatesAsync(rates, cancellationToken);

            _logger.LogInformation("Currency rates update completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update currency rates");
            return false;
        }
    }
} 