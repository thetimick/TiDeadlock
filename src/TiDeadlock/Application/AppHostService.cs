using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiDeadlock.Resources;
using TiDeadlock.Services.Config;
using TiDeadlock.Services.RunLoop;
using TiDeadlock.Services.Search;
using TiDeadlock.Services.Storage;
using TiDeadlock.Services.Update;
using TiDeadlock.ViewModels.Main;
using TiDeadlock.Windows.Main;

// ReSharper disable InvertIf

namespace TiDeadlock.Application;

public partial class AppHostService(IConfiguration configuration, IServiceProvider provider): IHostedService {
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (System.Windows.Application.Current.Windows.OfType<MainWindow>().Any())
            return;

        if (await PrepareRequiredServicesAsync() == false)
        {
            System.Windows.Application.Current.Shutdown();
            return;
        }

        if (configuration["service"] != "true")
        {
            // Программа запущена по умолчанию
            provider.GetService<MainViewModel>()?.OnLoaded();
            provider.GetService<MainWindow>()?.Show();
        }
        else
        {
            // Программа запущена как сервис
            provider.GetService<IRunLoopService>()?.RunAsync();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await provider.GetRequiredService<IStorageService>().SaveAsync();
    }
}

public partial class AppHostService
{
    private async Task<bool> PrepareRequiredServicesAsync()
    {
        // Проверяем обновления
        if (configuration["skipUpdate"] != "true") 
            await provider.GetRequiredService<IUpdateService>().UpdateAsync();
        
        await provider.GetRequiredService<IStorageService>().ObtainAsync();
        await provider.GetRequiredService<IConfigService>().ObtainAsync();
        
        // Получаем путь до папки с Deadlock
        if (await provider.GetRequiredService<ISearchService>().ObtainAsync() == null)
        {
            MessageBox.Show(
                AppLocalization.MessageBoxDescriptionUncorrectPath, 
                AppLocalization.MessageBoxErrorTitle, 
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
            
            System.Windows.Application.Current.Shutdown();
            return false;
        }
        
        return true;
    }
}