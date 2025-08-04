using System.ComponentModel.DataAnnotations;

namespace MigrationService.Tool.Configuration;

public class MigrationSettings
{
    public const string SectionName = "Migration";

    [Required]
    public DatabaseSettings Database { get; set; } = new();
}

public class DatabaseSettings
{
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    public string Host { get; set; } = "postgresql";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "currencytracker_dev";
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = "admin";

    public string BuildConnectionString()
    {
        if (!string.IsNullOrEmpty(ConnectionString))
            return ConnectionString;

        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}



 