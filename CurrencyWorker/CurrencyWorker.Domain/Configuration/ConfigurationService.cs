using Microsoft.Extensions.Configuration;

namespace CurrencyWorker.Domain.Configuration;

public class ConfigurationService
{
    public CurrencyApiConfiguration CurrencyApi { get; }
    public DatabaseConfiguration Database { get; }
    public WorkerConfiguration Worker { get; }
    
    public ConfigurationService(IConfiguration configuration)
    {
        CurrencyApi = configuration.GetSection("CurrencyApi").Get<CurrencyApiConfiguration>() 
            ?? new CurrencyApiConfiguration();
        Database = configuration.GetSection("Database").Get<DatabaseConfiguration>() 
            ?? new DatabaseConfiguration();
        Worker = configuration.GetSection("Worker").Get<WorkerConfiguration>() 
            ?? new WorkerConfiguration();
            
        // Read connection string from standard ConnectionStrings__DefaultConnection format
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            Database.ConnectionString = connectionString;
        }
    }
} 