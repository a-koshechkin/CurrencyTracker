namespace Shared.Infrastructure.Services;

public class PollyConfiguration
{
    public RetryConfiguration DatabaseRetry { get; set; } = new();
    public RetryConfiguration MigrationRetry { get; set; } = new();
}

public class RetryConfiguration
{
    public int MaxRetries { get; set; } = 10;
    public int BaseDelaySeconds { get; set; } = 2;
} 