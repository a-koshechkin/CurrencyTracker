using CurrencyWorker.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly.Retry;
using Shared.Domain.Entities;
using Shared.Infrastructure.Services;

namespace CurrencyWorker.Infrastructure.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
    private readonly string _connectionString;
    private readonly ILogger<CurrencyRepository> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public CurrencyRepository(
        IConfiguration configuration,
        ILogger<CurrencyRepository> logger,
        IOptions<PollyConfiguration>? pollyConfig = null)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured");
        _logger = logger;
        
        if (pollyConfig != null)
        {
            _retryPolicy = RetryPolicyFactory.CreateDatabaseRetryPolicy(pollyConfig, logger);
        }
        else
        {
            _retryPolicy = RetryPolicyFactory.CreateRetryPolicy(
                new RetryConfiguration { MaxRetries = 3, BaseDelaySeconds = 2 }, 
                logger, 
                "Database operation");
        }
    }

    public async Task UpdateCurrencyRatesAsync(IEnumerable<CurrencyRate> rates, CancellationToken cancellationToken = default)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var updatedCount = 0;
            var insertedCount = 0;

            foreach (var rate in rates)
            {
                try
                {
                    // Try to update existing currency by code
                    var rowsAffected = await UpdateExistingCurrencyAsync(connection, rate, cancellationToken);
                    
                    if (rowsAffected > 0)
                    {
                        updatedCount++;
                    }
                    else
                    {
                        // If no existing currency found, insert new one
                        await InsertNewCurrencyAsync(connection, rate, cancellationToken);
                        insertedCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process currency {CurrencyCode}", rate.CurrencyCode);
                }
            }

            _logger.LogInformation("Currency rates update completed: {UpdatedCount} updated, {InsertedCount} inserted", 
                updatedCount, insertedCount);
        });
    }

    private async Task<int> UpdateExistingCurrencyAsync(NpgsqlConnection connection, CurrencyRate rate, CancellationToken cancellationToken)
    {
        using var command = new NpgsqlCommand(
            "UPDATE currency SET rate = @rate WHERE name ILIKE @currencyCode", connection);
        
        command.Parameters.AddWithValue("rate", rate.Rate);
        command.Parameters.AddWithValue("currencyCode", $"%{rate.CurrencyCode}%");
        
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task InsertNewCurrencyAsync(NpgsqlConnection connection, CurrencyRate rate, CancellationToken cancellationToken)
    {
        using var command = new NpgsqlCommand(
            "INSERT INTO currency (name, rate) VALUES (@name, @rate)", connection);
        
        command.Parameters.AddWithValue("name", rate.CurrencyCode);
        command.Parameters.AddWithValue("rate", rate.Rate);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
} 