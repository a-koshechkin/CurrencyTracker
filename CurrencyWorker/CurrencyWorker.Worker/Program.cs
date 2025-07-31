using CurrencyWorker.Application.Services;
using CurrencyWorker.Domain.Interfaces;
using CurrencyWorker.Infrastructure.Repositories;
using CurrencyWorker.Infrastructure.Services;
using CurrencyWorker.Worker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register domain interfaces as singletons since they're used by hosted service
        services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
        services.AddSingleton<ICurrencyApiService, RussianCentralBankApiService>();
        
        // Register application services as singletons
        services.AddSingleton<CurrencyUpdateService>();
        
        // Register background worker
        services.AddHostedService<CurrencyUpdateWorker>();
        
        // Register HttpClient for API calls
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
