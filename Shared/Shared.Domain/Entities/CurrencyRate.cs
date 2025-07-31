namespace Shared.Domain.Entities;

public class CurrencyRate
{
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Rate { get; set; }
} 