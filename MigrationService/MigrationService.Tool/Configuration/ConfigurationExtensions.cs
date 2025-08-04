using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Configuration;
using Shared.Infrastructure.Services;

namespace MigrationService.Tool.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddMigrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MigrationSettings>(configuration.GetSection(MigrationSettings.SectionName));
        
        var migrationSettings = configuration.GetSection(MigrationSettings.SectionName).Get<MigrationSettings>() ?? new MigrationSettings();
        
        var connectionString = migrationSettings.Database.BuildConnectionString();
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured");
        }
        
        services.Configure<PollyConfiguration>(configuration.GetSection("Polly"));
        services.AddSingleton<DatabaseHealthService>(provider => 
            new DatabaseHealthService(
                connectionString, 
                provider.GetRequiredService<ILogger<DatabaseHealthService>>(),
                provider.GetRequiredService<IOptions<PollyConfiguration>>()));
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(ConfigurationExtensions).Assembly).For.Migrations())
            .AddLogging(lb => lb
                .AddFluentMigratorConsole());
        services.AddScoped<Services.MigrationRunner>();
        
        return services;
    }
} 