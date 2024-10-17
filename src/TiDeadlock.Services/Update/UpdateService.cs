using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TiDeadlock.Entities.Update;

namespace TiDeadlock.Services.Update;

public interface IUpdateService
{
    Task UpdateAsync();
}

public class UpdateService(ILogger<UpdateService> logger): IUpdateService
{
    private const string UpdateConfigUrl = "https://raw.githubusercontent.com/thetimick/PublicStorage/refs/heads/main/TD/Configs/update.json";
    private const string NewFileName = "new.exe";
    
    public async Task UpdateAsync()
    {
        logger.LogInformation("UpdateAsync - Start");
        
        var config = await ObtainUpdateConfigAsync();
        var appVersion = GetCurrentVersion();
        
        if (config is null || appVersion is null)
            return;
        
        logger.LogInformation("UpdateAsync - RemoteConfig = {RemoteUpdateConfig}, AppVersion = {AppVersion}", config, appVersion);

        if (appVersion < config.CurrentVersion.Version && config.CurrentVersion.Link != string.Empty)
        {
            await ObtainFile(config.CurrentVersion.Link);
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, NewFileName)))
                RestartApp();
        }
        
        logger.LogInformation("UpdateAsync - Stop");
    }

    private static async Task<UpdateEntity?> ObtainUpdateConfigAsync()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(UpdateConfigUrl);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UpdateEntity>(content);
    }

    private static async Task ObtainFile(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        var bytes = await response.Content.ReadAsByteArrayAsync();
        await File.WriteAllBytesAsync(Path.Combine(AppContext.BaseDirectory, NewFileName), bytes);
    }

    private static void RestartApp()
    {
        var currentFileName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
        var command = $"""
                       taskkill /IM "{currentFileName}" /F && timeout 1 && del "{currentFileName}" && rename "{NewFileName}" "{currentFileName}" && start "" "{currentFileName}"
                       """;
        Process.Start(
            new ProcessStartInfo("cmd.exe", "/C " + command)
            {
                WorkingDirectory = AppContext.BaseDirectory,
                CreateNoWindow = true,
                UseShellExecute = false
            }
        );
    }

    private static double? GetCurrentVersion()
    {
        if (Assembly.GetExecutingAssembly().GetName().Version is { } version)
            return double.Parse($"{version.Major},{version.Minor}");
        return null;
    }
}