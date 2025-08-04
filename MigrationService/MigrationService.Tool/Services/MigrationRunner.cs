using FluentMigrator.Runner;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Shared.Infrastructure.Services;

namespace MigrationService.Tool.Services;

public class MigrationRunner(
    IMigrationRunner migrationRunner,
    DatabaseHealthService databaseHealthService,
    ILogger<MigrationRunner> logger,
    IOptions<PollyConfiguration> pollyConfig)
{
    private readonly IMigrationRunner _migrationRunner = migrationRunner;
    private readonly DatabaseHealthService _databaseHealthService = databaseHealthService;
    private readonly ILogger<MigrationRunner> _logger = logger;
    private readonly AsyncRetryPolicy _migrationRetryPolicy = RetryPolicyFactory.CreateMigrationRetryPolicy(pollyConfig, logger);

    public async Task<bool> RunMigrationsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting migration process...");

            var isDatabaseReady = await _databaseHealthService.WaitForDatabaseAsync(cancellationToken);
            if (!isDatabaseReady)
            {
                _logger.LogError("Database is not ready after all retry attempts");
                return false;
            }

            var databaseCreated = await _databaseHealthService.CreateDatabaseIfNotExistsAsync(cancellationToken);
            if (!databaseCreated)
            {
                _logger.LogError("Failed to create database");
                return false;
            }

            await _migrationRetryPolicy.ExecuteAsync(() =>
            {
                _logger.LogInformation("Running migrations...");
                _migrationRunner.MigrateUp();
                _logger.LogInformation("Migrations completed successfully!");
                return Task.CompletedTask;
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration process failed");
            return false;
        }
    }

    public async Task<bool> RollbackMigrationAsync(long version, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting rollback to version {Version}...", version);

            var isDatabaseReady = await _databaseHealthService.WaitForDatabaseAsync(cancellationToken);
            if (!isDatabaseReady)
            {
                _logger.LogError("Database is not ready for rollback");
                return false;
            }

            await _migrationRetryPolicy.ExecuteAsync(() =>
            {
                _logger.LogInformation("Rolling back to version {Version}...", version);
                _migrationRunner.MigrateDown(version);
                _logger.LogInformation("Rollback to version {Version} completed successfully!", version);
                return Task.CompletedTask;
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rollback process failed");
            return false;
        }
    }
}
