using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TiDeadlock.Application.AppHost;
using TiDeadlock.Services;
using TiDeadlock.ViewModel.Main;
using TiDeadlock.Windows.Main;

namespace TiDeadlock.Application;

public partial class App
{
    private static readonly IHost AppHost = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(
            builder =>
            {
                builder.SetBasePath(AppContext.BaseDirectory);
                builder.AddCommandLine(Environment.GetCommandLineArgs());
            }
        )
        .ConfigureServices(
            collection =>
            {
                collection.AddHostedService<ApplicationHostService>();
                
                collection.AddSingleton<IStorageService, StorageService>();
                collection.AddSingleton<ISearchService, SearchService>();
                collection.AddSingleton<ILocalizationService, LocalizationService>();
                
                collection.AddTransient<MainWindow>();
                collection.AddTransient<MainViewModel>();
            }
        )
        .Build();
    
    private void OnStartup(object sender, StartupEventArgs e)
    {
        AppHost.Start();
    }
    
    private void OnExit(object sender, ExitEventArgs e)
    {
        AppHost.StopAsync()
            .Wait();
        AppHost.Dispose();
    }
    
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            $"{e.Exception.Message}\n\n{string.Join("", e.Exception.StackTrace?.Take(1000) ?? [])}", 
            "FatalError!", 
            MessageBoxButton.OK, 
            MessageBoxImage.Error
        );
        
        e.Handled = true;
    }
}