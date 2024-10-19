using System.Diagnostics;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using TiDeadlock.Services.Storage;

// ReSharper disable InvertIf

namespace TiDeadlock.Services.Search;

public interface ISearchService
{
    string? Cached { get; }
    bool IsCached { get; }
    
    Task<string?> ObtainAsync();
}

public class SearchService(IStorageService storage): ISearchService
{
    public string? Cached { get; private set; }
    public bool IsCached => Cached != null;
    
    public async Task<string?> ObtainAsync()
    {
        // Если есть данные в кеше - отдаем
        if (IsCached)
            return Cached;

        // Если есть корректный путь в storage - отдаем
        if (CheckExecutable(storage.Cached?.Path, storage.Cached?.DeadlockExecutable))
        {
            Cached = storage.Cached?.Path;
            return Cached;
        }

        // Сохраненного пути не найдено - ищем его в libraryfolders.vdf
        var path = await GetPathFromLibraryFolders();
        if (CheckExecutable(path, storage.Cached?.DeadlockExecutable))
        {
            Cached = path;
            if (storage.Cached != null) 
                storage.Cached.Path = Cached;
            return Cached;
        }
        
        // Крайний вариант - просим пользователя указать путь вручную
        return OpenFolder();
    }

    private static bool CheckExecutable(string? path, string? executable)
    {
        if (path is null || executable is null)
            return false;
        var fullPath = Path.Combine(path, executable);
        return File.Exists(fullPath);
    }

    private string? GetSteamPath()
    {
        // Дефолтный путь
        var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
        if (CheckExecutable(defaultPath, storage.Cached?.SteamExecutable))
            return defaultPath;

        // Путь из запущенные процессов
        var processPath = Path.GetDirectoryName(Process.GetProcesses().FirstOrDefault(process => process.ProcessName == "steam")?.MainModule?.FileName);
        return CheckExecutable(processPath, storage.Cached?.SteamExecutable) 
            ? processPath 
            : null;
    }

    private async Task<string?> GetPathFromLibraryFolders()
    {
        var steamDirectory = GetSteamPath();
        if (steamDirectory == null)
            return null;
        
        VToken? result = null;
        
        var vdfString = await File.ReadAllTextAsync(Path.Combine(steamDirectory, "steamapps", "libraryfolders.vdf"));
        var vdfDeserialized = VdfConvert.Deserialize(vdfString);
        for (var index = 0;; index++)
        {
            var token = vdfDeserialized.Value[index.ToString()];
            if (token is null)
                break;

            var subToken = token["apps"]?.Value<VToken>()["1422450"];
            if (subToken is null)
                continue;

            result = token["path"];
            break;
        }
        
        return result != null 
            ? Path.Combine(result.Value<string>(), "steamapps", "common", "Deadlock") 
            : null;
    }

    private string? OpenFolder()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Пожалуйста, укажите путь к папке с Deadlock",
            Multiselect = false
        };
        
        return dialog.ShowDialog() == true && CheckExecutable(dialog.FolderName, storage.Cached?.DeadlockExecutable) 
            ? dialog.FolderName 
            : null;
    }
}