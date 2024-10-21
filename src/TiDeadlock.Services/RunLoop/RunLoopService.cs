using System.Diagnostics;
using System.Reflection;
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
        var storage = await storageService.ObtainAsync();
        
        if (!useEnglishForHeroes && !useEnglishForItems || storage.IsServiceInstalled)
            return;

        using var ts = new TaskService();
        var task = ts.NewTask();

        var fileName = Environment.GetCommandLineArgs()[0]
            .Replace(".dll", ".exe");
            
        task.Triggers.Add(new LogonTrigger());
        task.Actions.Add(
            new ExecAction(
                fileName, 
                $"--service=true --useEnglishForHeroes={useEnglishForHeroes.ToString().ToLower()} --useEnglishForItems={useEnglishForItems.ToString().ToLower()}", 
                Environment.CurrentDirectory
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
    }

    public async Task UninstallAsync()
    {
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
    }

    public async Task RunAsync()
    {
        while (true)
        {
            if (configuration["useEnglishForHeroes"] != "true" || configuration["useEnglishForItems"] != "true")
            {
                Application.Current.Shutdown();
                break;
            }
            
            var process = Process.GetProcessesByName("project8").FirstOrDefault();
            if (process == null)
            {
                Thread.Sleep(1500);
                continue;
            }
            
            if (configuration["useEnglishForHeroes"] == "true")
                await localizationService.ChangeLocalizationForHeroesAsync();
            if (configuration["useEnglishForItems"] == "true")
                await localizationService.ChangeLocalizationForItemsAsync();
            
            await process.WaitForExitAsync();
            
            await localizationService.RestoreAsync();
        }
    }
}