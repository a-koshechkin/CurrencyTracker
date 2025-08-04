namespace CurrencyWorker.Domain.Configuration;

public class DatabaseConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int BaseDelaySeconds { get; set; } = 2;
    public int MigrationRetryMaxRetries { get; set; } = 5;
    public int MigrationRetryBaseDelaySeconds { get; set; } = 1;

    public string BuildConnectionString()
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured. Ensure ConnectionStrings__DefaultConnection environment variable is set.");
        }
        
        return ConnectionString;
    }
} 