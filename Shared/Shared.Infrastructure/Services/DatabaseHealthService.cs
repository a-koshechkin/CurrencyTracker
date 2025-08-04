using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly.Retry;
using Shared.Infrastructure.Configuration;

namespace Shared.Infrastructure.Services;

public class DatabaseHealthService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseHealthService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public DatabaseHealthService(
        IConfiguration configuration,
        ILogger<DatabaseHealthService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured");
        _logger = logger;
        
        var retryConfig = new RetryConfiguration();
        _retryPolicy = RetryPolicyFactory.CreateRetryPolicy(retryConfig, logger, "Database connection");
    }

    public DatabaseHealthService(
        string connectionString, 
        ILogger<DatabaseHealthService> logger,
        IOptions<PollyConfiguration>? pollyConfig = null)
    {
        _connectionString = connectionString;
        _logger = logger;
        
        if (pollyConfig != null)
        {
            _retryPolicy = RetryPolicyFactory.CreateDatabaseRetryPolicy(pollyConfig, logger);
        }
        else
        {
            var retryConfig = new RetryConfiguration();
            _retryPolicy = RetryPolicyFactory.CreateRetryPolicy(retryConfig, logger, "Database connection");
        }
    }

    public async Task<bool> WaitForDatabaseAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Waiting for database to be ready...");
        
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);
                
                using var command = new NpgsqlCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync(cancellationToken);
                
                _logger.LogInformation("Database is ready!");
            });
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to database after all retry attempts");
            return false;
        }
    }

    public async Task<bool> CheckDatabaseExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.Database;
            
            builder.Database = "postgres";
            using var connection = new NpgsqlConnection(builder.ToString());
            await connection.OpenAsync(cancellationToken);
            
            using var command = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = @databaseName", connection);
            command.Parameters.AddWithValue("databaseName", databaseName);
            
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if database exists");
            return false;
        }
    }

    public async Task<bool> CreateDatabaseIfNotExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.Database;
            
            if (string.IsNullOrEmpty(databaseName))
            {
                _logger.LogError("Database name is not specified in connection string");
                return false;
            }
            
            builder.Database = "postgres";
            using var connection = new NpgsqlConnection(builder.ToString());
            await connection.OpenAsync(cancellationToken);
            
            using var checkCommand = new NpgsqlCommand(
                "SELECT 1 FROM pg_database WHERE datname = @databaseName", connection);
            checkCommand.Parameters.AddWithValue("databaseName", databaseName);
            
            var exists = await checkCommand.ExecuteScalarAsync(cancellationToken);
            
            if (exists == null)
            {
                _logger.LogInformation("Creating database {DatabaseName}", databaseName);
                
                using var createCommand = new NpgsqlCommand(
                    $"CREATE DATABASE \"{databaseName}\"", connection);
                await createCommand.ExecuteNonQueryAsync(cancellationToken);
                
                _logger.LogInformation("Database {DatabaseName} created successfully", databaseName);
                return true;
            }
            
            _logger.LogInformation("Database {DatabaseName} already exists", databaseName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database: {Error}", ex.Message);
            return false;
        }
    }
} 