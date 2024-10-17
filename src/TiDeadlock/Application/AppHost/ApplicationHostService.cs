using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiDeadlock.Services;
using TiDeadlock.Services.Update;
using TiDeadlock.ViewModels.Main;
using TiDeadlock.Windows.Main;

namespace TiDeadlock.Application.AppHost;

public class ApplicationHostService(IServiceProvider provider): IHostedService {
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (System.Windows.Application.Current.Windows.OfType<MainWindow>().Any())
            return Task.CompletedTask;

        Task.Factory.StartNew(
            async () =>
            {
                if (provider.GetService<IConfiguration>()?["skipUpdate"] != "true") 
                    await provider.GetRequiredService<IUpdateService>().UpdateAsync();
                
                await provider.GetRequiredService<IStorageService>().ObtainAsync();
                
                await System.Windows.Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal, 
                    () =>
                    {
                        provider.GetService<MainViewModel>()?.OnLoaded();
                        provider.GetService<MainWindow>()?.Show();
                    }
                );
            }, 
            cancellationToken
        );
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        provider.GetRequiredService<IStorageService>().SaveAsync();
        return Task.CompletedTask;
    }
}