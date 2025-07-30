using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;
using Polly.Retry;

namespace MigrationService.Tool.Services;

public static class RetryPolicyFactory
{
    public static AsyncRetryPolicy CreateDatabaseRetryPolicy<T>(
        IOptions<PollyConfiguration> pollyConfig,
        ILogger<T> logger)
    {
        var config = pollyConfig.Value.DatabaseRetry;
        
        return Policy
            .Handle<NpgsqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: config.MaxRetries,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(config.BaseDelaySeconds * Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "Database operation attempt {RetryCount}/{MaxRetries} failed. Retrying in {Delay}ms. Error: {Error}",
                        retryCount, config.MaxRetries, timeSpan.TotalMilliseconds, exception.Message);
                });
    }

    public static AsyncRetryPolicy CreateMigrationRetryPolicy<T>(
        IOptions<PollyConfiguration> pollyConfig,
        ILogger<T> logger)
    {
        var config = pollyConfig.Value.MigrationRetry;
        
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: config.MaxRetries,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(config.BaseDelaySeconds * Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "Migration attempt {RetryCount}/{MaxRetries} failed. Retrying in {Delay}ms. Error: {Error}",
                        retryCount, config.MaxRetries, timeSpan.TotalMilliseconds, exception.Message);
                });
    }
} 