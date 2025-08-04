using CurrencyWorker.Domain.Configuration;
using CurrencyWorker.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Shared.Domain.Entities;
using Shared.Infrastructure.Services;
using System.Text;
using System.Xml.Linq;

namespace CurrencyWorker.Infrastructure.Services;

public class RussianCentralBankApiService : ICurrencyApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RussianCentralBankApiService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly CurrencyApiConfiguration _config;

    public RussianCentralBankApiService(
        HttpClient httpClient,
        ILogger<RussianCentralBankApiService> logger,
        ConfigurationService configurationService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _config = configurationService.CurrencyApi;
        
        _retryPolicy = RetryPolicyFactory.CreateRetryPolicy(
            new RetryConfiguration { MaxRetries = _config.MaxRetries, BaseDelaySeconds = _config.BaseDelaySeconds }, 
            logger, 
            "API call");
    }

    static RussianCentralBankApiService()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<IEnumerable<CurrencyRate>> GetCurrentRatesAsync(CancellationToken cancellationToken = default)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            _logger.LogInformation("Fetching currency rates from Russian Central Bank API...");

            var response = await _httpClient.GetAsync(_config.BaseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            
            var encoding = Encoding.GetEncoding(_config.Encoding);
            var responseText = encoding.GetString(responseBytes);
            
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
        });
    }

    private static decimal ParseRate(string? value, string? nominal)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        if (string.IsNullOrEmpty(nominal))
            return 0;

        var normalizedValue = value.Replace(',', '.');
        
        if (decimal.TryParse(normalizedValue, out var rate) && decimal.TryParse(nominal, out var nominalValue))
            return rate / nominalValue;

        return 0;
    }
} 