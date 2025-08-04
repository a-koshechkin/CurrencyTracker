using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MigrationService.Tool.Services;
using MigrationService.Tool.Configuration;

var host = CreateHostBuilder(args).Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

if (args.Contains("--migrate"))
{
    await RunMigrationsAsync(host, logger);
}
else if (args.Contains("--rollback"))
{
    var version = args.FirstOrDefault(arg => arg.StartsWith("--version="))?.Split('=')[1];
    if (long.TryParse(version, out var rollbackVersion))
    {
        await RollbackMigrationAsync(host, rollbackVersion, logger);
    }
    else
    {
        logger.LogError("Please specify version to rollback: --rollback --version=003");
        Environment.Exit(1);
    }
}
else
{
    logger.LogInformation("Usage:");
    logger.LogInformation("  --migrate                    Run all pending migrations");
    logger.LogInformation("  --rollback --version=X       Rollback to specific version");
    Environment.Exit(1);
}

static async Task RunMigrationsAsync(IHost host, ILogger logger)
{
    using var scope = host.Services.CreateScope();
    var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();

    var success = await migrationRunner.RunMigrationsAsync();
    if (!success)
    {
        logger.LogError("Migration failed");
        Environment.Exit(1);
    }
    logger.LogInformation("Migration completed successfully");
}

static async Task RollbackMigrationAsync(IHost host, long version, ILogger logger)
{
    using var scope = host.Services.CreateScope();
    var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
    
    var success = await migrationRunner.RollbackMigrationAsync(version);
    if (!success)
    {
        logger.LogError("Rollback failed");
        Environment.Exit(1);
    }
    logger.LogInformation("Rollback completed successfully");
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddMigrationServices(context.Configuration);
        });
}