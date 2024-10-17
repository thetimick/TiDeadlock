using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TiDeadlock.Application.AppHost;
using TiDeadlock.Services;
using TiDeadlock.Services.Update;
using TiDeadlock.Windows.Main;
using MainViewModel = TiDeadlock.ViewModels.Main.MainViewModel;

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
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.File("TiDeadlock.log")
                    .CreateLogger();
                
                collection.AddLogging(builder => builder.AddSerilog(dispose: true));
                
                collection.AddHostedService<ApplicationHostService>();
                
                collection.AddSingleton<IUpdateService, UpdateService>();
                
                collection.AddSingleton<IStorageService, StorageService>();
                collection.AddSingleton<ISearchService, SearchService>();
                collection.AddSingleton<ILocalizationService, LocalizationService>();
                
                collection.AddSingleton<MainViewModel>();
                collection.AddSingleton<MainWindow>();
            }
        )
        .Build();
    
    private void OnStartup(object sender, StartupEventArgs e)
    {
        AppHost.Start();
    }
    
    private void OnExit(object sender, ExitEventArgs e)
    {
        AppHost.StopAsync().Wait();
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
}