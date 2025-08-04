namespace CurrencyWorker.Domain.Configuration;

public class CurrencyApiConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int BaseDelaySeconds { get; set; } = 2;
    public int TimeoutSeconds { get; set; } = 30;
    public string Encoding { get; set; } = "windows-1251";
} 