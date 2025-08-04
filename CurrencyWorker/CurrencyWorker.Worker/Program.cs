using CurrencyWorker.Application.Services;
using CurrencyWorker.Domain.Configuration;
using CurrencyWorker.Domain.Interfaces;
using CurrencyWorker.Infrastructure.Repositories;
using CurrencyWorker.Infrastructure.Services;
using CurrencyWorker.Worker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ConfigurationService>();
        
        services.AddSingleton<ICurrencyRepository, CurrencyRepository>();
        services.AddSingleton<ICurrencyApiService, RussianCentralBankApiService>();
        
        services.AddSingleton<CurrencyUpdateService>();
        
        services.AddHostedService<CurrencyUpdateWorker>();
        
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
