using CurrencyWorker.Application.Services;
using CurrencyWorker.Domain.Configuration;

namespace CurrencyWorker.Worker;

public class CurrencyUpdateWorker : BackgroundService
{
    private readonly CurrencyUpdateService _currencyUpdateService;
    private readonly ILogger<CurrencyUpdateWorker> _logger;
    private readonly TimeSpan _updateInterval;

    public CurrencyUpdateWorker(
        CurrencyUpdateService currencyUpdateService,
        ILogger<CurrencyUpdateWorker> logger,
        ConfigurationService configurationService)
    {
        _currencyUpdateService = currencyUpdateService;
        _logger = logger;
        
        var config = configurationService.Worker;
        _updateInterval = TimeSpan.FromMinutes(config.UpdateIntervalMinutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Currency Update Worker started. Update interval: {Interval}", _updateInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scheduled currency rates update...");
                
                var success = await _currencyUpdateService.UpdateCurrencyRatesAsync(stoppingToken);
                
                if (success)
                {
                    _logger.LogInformation("Currency rates update completed successfully");
                }
                else
                {
                    _logger.LogWarning("Currency rates update failed");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Currency Update Worker is stopping...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during currency rates update");
            }

            await Task.Delay(_updateInterval, stoppingToken);
        }

        _logger.LogInformation("Currency Update Worker stopped");
    }
} 