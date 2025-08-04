namespace CurrencyWorker.Domain.Configuration;

public class WorkerConfiguration
{
    public int UpdateIntervalMinutes { get; set; } = 60;
    public bool EnableDetailedLogging { get; set; } = false;
} 