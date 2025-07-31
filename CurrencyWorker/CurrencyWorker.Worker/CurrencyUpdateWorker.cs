using CurrencyWorker.Application.Services;

namespace CurrencyWorker.Worker;

public class CurrencyUpdateWorker : BackgroundService
{
    private readonly CurrencyUpdateService _currencyUpdateService;
    private readonly ILogger<CurrencyUpdateWorker> _logger;
    private readonly TimeSpan _updateInterval;

    public CurrencyUpdateWorker(
        CurrencyUpdateService currencyUpdateService,
        ILogger<CurrencyUpdateWorker> logger,
        IConfiguration configuration)
    {
        _currencyUpdateService = currencyUpdateService;
        _logger = logger;
        
        // Get update interval from configuration (default: 1 hour)
        var intervalMinutes = configuration.GetValue<int>("CurrencyUpdate:IntervalMinutes", 60);
        _updateInterval = TimeSpan.FromMinutes(intervalMinutes);
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

            // Wait for next update interval
            await Task.Delay(_updateInterval, stoppingToken);
        }

        _logger.LogInformation("Currency Update Worker stopped");
    }
} 