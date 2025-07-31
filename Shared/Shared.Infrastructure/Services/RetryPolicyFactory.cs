using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Shared.Infrastructure.Services;

public static class RetryPolicyFactory
{
    public static AsyncRetryPolicy CreateRetryPolicy<T>(
        RetryConfiguration config,
        ILogger<T> logger,
        string operationType = "Operation")
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: config.MaxRetries,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(config.BaseDelaySeconds * Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "{OperationType} attempt {RetryCount}/{MaxRetries} failed. Retrying in {Delay}ms. Error: {Error}",
                        operationType, retryCount, config.MaxRetries, timeSpan.TotalMilliseconds, exception.Message);
                });
    }

    public static AsyncRetryPolicy CreateDatabaseRetryPolicy<T>(
        IOptions<PollyConfiguration> pollyConfig,
        ILogger<T> logger)
    {
        var config = pollyConfig.Value.DatabaseRetry;
        return CreateRetryPolicy(config, logger, "Database connection");
    }

    public static AsyncRetryPolicy CreateMigrationRetryPolicy<T>(
        IOptions<PollyConfiguration> pollyConfig,
        ILogger<T> logger)
    {
        var config = pollyConfig.Value.MigrationRetry;
        return CreateRetryPolicy(config, logger, "Migration");
    }
} 