using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationService.Tool.Services;
using MigrationService.Tool.Configuration;

var host = CreateHostBuilder(args).Build();

if (args.Contains("--migrate"))
{
    await RunMigrationsAsync(host);
}
else if (args.Contains("--rollback"))
{
    var version = args.FirstOrDefault(arg => arg.StartsWith("--version="))?.Split('=')[1];
    if (long.TryParse(version, out var rollbackVersion))
    {
        await RollbackMigrationAsync(host, rollbackVersion);
    }
    else
    {
        Console.WriteLine("Please specify version to rollback: --rollback --version=003");
        Environment.Exit(1);
    }
}
else
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  --migrate                    Run all pending migrations");
    Console.WriteLine("  --rollback --version=X       Rollback to specific version");
    Environment.Exit(1);
}

static async Task RunMigrationsAsync(IHost host)
{
    using var scope = host.Services.CreateScope();
    var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();

    var success = await migrationRunner.RunMigrationsAsync();
    if (!success)
    {
        Environment.Exit(1);
    }
}

static async Task RollbackMigrationAsync(IHost host, long version)
{
    using var scope = host.Services.CreateScope();
    var migrationRunner = scope.ServiceProvider.GetRequiredService<MigrationRunner>();
    
    var success = await migrationRunner.RollbackMigrationAsync(version);
    if (!success)
    {
        Environment.Exit(1);
    }
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddMigrationServices(context.Configuration);
        });
}