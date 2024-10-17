using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiDeadlock.Services;
using TiDeadlock.Windows.Main;

namespace TiDeadlock.Application.AppHost;

public class ApplicationHostService(
    IServiceProvider serviceProvider, 
    IConfiguration configuration,
    ILocalizationService localizationService
): IHostedService {
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (System.Windows.Application.Current.Windows.OfType<MainWindow>().Any())
            return Task.CompletedTask;
        
        serviceProvider
            .GetRequiredService<IStorageService>()
            .Obtain();

        if (configuration["hide"] == "true")
        {
            if (configuration["useEnglishForHeroes"] == "true")
                localizationService.ChangeLocalizationForHeroes(LocalizationService.Localization.English);
            if (configuration["useEnglishForItems"] == "true")
                localizationService.ChangeLocalizationForItems(LocalizationService.Localization.English);
            
            System.Windows.Application.Current.Shutdown();
            
            return Task.CompletedTask;
        }
        
        serviceProvider
            .GetRequiredService<MainWindow>()
            .Show();
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        serviceProvider
            .GetRequiredService<IStorageService>()
            .Save();
        
        return Task.CompletedTask;
    }
}