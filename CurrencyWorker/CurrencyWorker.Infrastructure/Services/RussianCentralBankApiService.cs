using CurrencyWorker.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Shared.Domain.Entities;
using System.Xml.Linq;
using System.Text;

namespace CurrencyWorker.Infrastructure.Services;

public class RussianCentralBankApiService(
    HttpClient httpClient,
    ILogger<RussianCentralBankApiService> logger) : ICurrencyApiService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<RussianCentralBankApiService> _logger = logger;
    private const string ApiUrl = "http://www.cbr.ru/scripts/XML_daily.asp";

    static RussianCentralBankApiService()
    {
        // Register the CodePages encoding provider to support windows-1251
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<IEnumerable<CurrencyRate>> GetCurrentRatesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching currency rates from Russian Central Bank API...");

            // Get response as bytes to handle encoding manually
            var response = await _httpClient.GetAsync(ApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            
            // Decode using windows-1251 encoding
            var responseText = Encoding.GetEncoding(1251).GetString(responseBytes);
            
            var xmlDoc = XDocument.Parse(responseText);

            var rates = xmlDoc.Descendants("Valute")
                .Select(valute => new CurrencyRate
                {
                    CurrencyCode = valute.Element("CharCode")?.Value ?? string.Empty,
                    Rate = ParseRate(valute.Element("Value")?.Value, valute.Element("Nominal")?.Value)
                })
                .Where(rate => !string.IsNullOrEmpty(rate.CurrencyCode) && rate.Rate > 0)
                .ToList();

            _logger.LogInformation("Successfully fetched {Count} currency rates from Russian Central Bank", rates.Count);
            return rates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch currency rates from Russian Central Bank API");
            return [];
        }
    }

    private static decimal ParseRate(string? value, string? nominal)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        if (string.IsNullOrEmpty(nominal))
            return 0;

        // Russian Central Bank uses comma as decimal separator
        var normalizedValue = value.Replace(',', '.');
        
        if (decimal.TryParse(normalizedValue, out var rate) && decimal.TryParse(nominal, out var nominalValue))
            return rate / nominalValue;

        return 0;
    }
} 