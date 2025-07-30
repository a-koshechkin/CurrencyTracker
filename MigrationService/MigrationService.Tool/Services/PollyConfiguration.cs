namespace MigrationService.Tool.Services;

public class PollyConfiguration
{
    public DatabaseRetryConfig DatabaseRetry { get; set; } = new();
    public MigrationRetryConfig MigrationRetry { get; set; } = new();
}

public class DatabaseRetryConfig
{
    public int MaxRetries { get; set; } = 10;
    public int BaseDelaySeconds { get; set; } = 2;
}

public class MigrationRetryConfig
{
    public int MaxRetries { get; set; } = 3;
    public int BaseDelaySeconds { get; set; } = 2;
} 