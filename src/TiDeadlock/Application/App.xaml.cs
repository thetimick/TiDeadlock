using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TiDeadlock.Services;
using TiDeadlock.Services.Config;
using TiDeadlock.Services.Localization;
using TiDeadlock.Services.RunLoop;
using TiDeadlock.Services.Search;
using TiDeadlock.Services.Storage;
using TiDeadlock.Services.Update;
using TiDeadlock.ViewModels.Main;
using TiDeadlock.Windows.Main;

namespace TiDeadlock.Application;

public partial class App
{
    public static readonly IHost AppHost = Host.CreateDefaultBuilder()
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
                ConfigureLogging();
                collection.AddLogging(builder => builder.AddSerilog(dispose: true));
                
                collection.AddHostedService<AppHostService>();
                
                collection.AddSingleton<IUpdateService, UpdateService>();
                collection.AddSingleton<IConfigService, ConfigService>();
                collection.AddSingleton<IStorageService, StorageService>();
                collection.AddSingleton<IRunLoopService, RunLoopService>();
                collection.AddSingleton<ISearchService, SearchService>();
                collection.AddSingleton<ILocalizationService, LocalizationService>();
                
                collection.AddSingleton<MainViewModel>();
                collection.AddSingleton<MainWindow>();
            }
        )
        .Build();
    
    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await AppHost.StartAsync();
    }
    
    private async void OnExit(object sender, ExitEventArgs e)
    {
        await AppHost.StopAsync();
        AppHost.Dispose();
    }
    
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        AppHost.Services.GetService<ILogger>()?.Fatal(e.Exception, "FatalError!");
        
        MessageBox.Show(
            $"{e.Exception.Message}\n\n{string.Join("", e.Exception.StackTrace?.Take(800) ?? [])}", 
            "FatalError!", 
            MessageBoxButton.OK, 
            MessageBoxImage.Error
        );

        e.Handled = true;
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File("TiDeadlock.log")
            .CreateLogger();
    }
}