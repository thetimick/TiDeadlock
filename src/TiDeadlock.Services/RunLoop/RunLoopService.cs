using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.TaskScheduler;
using TiDeadlock.Services.Localization;
using TiDeadlock.Services.Storage;
using Task = System.Threading.Tasks.Task;

namespace TiDeadlock.Services.RunLoop;

public interface IRunLoopService
{
    Task<bool> IsInstalledAsync();
    Task InstallAsync(bool useEnglishForHeroes, bool useEnglishForItems);
    Task UninstallAsync();
    
    Task RunAsync();
}

public class RunLoopService(
    ILogger<RunLoopService> logger,
    IConfiguration configuration,
    ILocalizationService localizationService,
    IStorageService storageService
): IRunLoopService {
    private const string ServiceName = "TiDeadlock.Service";
    
    public async Task<bool> IsInstalledAsync()
    {
        return (await storageService.ObtainAsync()).IsServiceInstalled;
    }

    public async Task InstallAsync(bool useEnglishForHeroes, bool useEnglishForItems)
    {
        logger.LogInformation("[InstallAsync] Starting...");
        
        var storage = await storageService.ObtainAsync();
        
        if (!useEnglishForHeroes && !useEnglishForItems || storage.IsServiceInstalled)
            return;

        using var ts = new TaskService();
        var task = ts.NewTask();
        
        task.Triggers.Add(new LogonTrigger());
        task.Actions.Add(
            new ExecAction(
                Environment.GetCommandLineArgs()[0], 
                $"--service=true --useEnglishForHeroes={useEnglishForHeroes.ToString().ToLower()} --useEnglishForItems={useEnglishForItems.ToString().ToLower()}", 
                AppContext.BaseDirectory
            )
        );
        task.Settings.Hidden = true;

        ts.RootFolder.RegisterTaskDefinition(ServiceName, task);
        ts.RootFolder.GetTasks()[ServiceName].Run();
        
        if (storageService.Cached != null)
        {
            storageService.Cached.IsServiceInstalled = true;
            await storageService.SaveAsync();
        }
        
        logger.LogInformation("[InstallAsync] Finished.");
    }

    public async Task UninstallAsync()
    {
        logger.LogInformation("[UninstallAsync] Starting...");
        
        var storage = await storageService.ObtainAsync();
        if (!storage.IsServiceInstalled)
            return;
        
        using var ts = new TaskService();
        ts.RootFolder.DeleteTask(ServiceName);

        if (storageService.Cached != null)
        {
            storageService.Cached.IsServiceInstalled = false;
            await storageService.SaveAsync();
        }
        
        logger.LogInformation("[UninstallAsync] Finished.");
    }

    public async Task RunAsync()
    {
        logger.LogInformation("[RunAsync] Starting...");
        
        while (true)
        {
            if (configuration["useEnglishForHeroes"] != "true" && configuration["useEnglishForItems"] != "true")
            {
                logger.LogInformation("[RunAsync] useEnglishForHeroes && useEnglishForItems is false... Shutdown.");
                
                Application.Current.Shutdown();
                break;
            }
            
            var process = Process.GetProcessesByName("project8").FirstOrDefault();
            if (process == null)
            {
                Thread.Sleep(1500);
                continue;
            }
            
            logger.LogInformation("[RunaAsync] Process is running... Patch...");
            
            if (configuration["useEnglishForHeroes"] == "true")
                await localizationService.ChangeLocalizationForHeroesAsync();
            if (configuration["useEnglishForItems"] == "true")
                await localizationService.ChangeLocalizationForItemsAsync();
            
            logger.LogInformation("[RunaAsync] Patch is success... Waiting...");
            
            await process.WaitForExitAsync();
            
            logger.LogInformation("[RunaAsync] Process is exit... Restore...");
            
            await localizationService.RestoreAsync();
        }
    }
}