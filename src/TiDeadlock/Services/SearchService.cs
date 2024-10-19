using System.Diagnostics;
using System.IO;
using System.Windows;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using Microsoft.Win32;
using TiDeadlock.Resources;
using TiDeadlock.Services.Storage;

namespace TiDeadlock.Services;

public interface ISearchService
{
    string? GetPathForDeadlock();
}

public class SearchService(IStorageService storage): ISearchService
{
    private string? _cachedPath;
    
    public string? GetPathForDeadlock()
    {
        // Если есть данные в кеше - отдаем
        if (_cachedPath is not null)
            return _cachedPath;

        // Если есть корректный путь в storage - отдаем
        if (CheckExecutable(storage.Cached?.Path, storage.Cached?.DeadlockExecutable))
        {
            _cachedPath = storage.Cached?.Path;
            return _cachedPath;
        }
        
        try
        {
            // Нашли директорию Steam, пытаемся вытащить путь оттуда
            var steamDirectory = GetSteamPath();
            if (steamDirectory is null)
                throw new DirectoryNotFoundException("Не удалось найти директорию Steam!\nПожалуйста, запустите его, перед запуском программы...");
            
            VToken? result = null;
            var vdfString = File.ReadAllText(Path.Combine(steamDirectory, "steamapps", "libraryfolders.vdf"));
            var vdfDeserialized = VdfConvert.Deserialize(vdfString);
            for (var index = 0;; index++)
            {
                var token = vdfDeserialized.Value[index.ToString()];
                if (token is null)
                    break;

                try
                {
                    var subToken = token["apps"]?.Value<VToken>()["1422450"];
                    if (subToken is null)
                        continue;

                    result = token["path"];
                    break;
                }
                catch
                {
                    // ignored
                }
            }
            ArgumentNullException.ThrowIfNull(result);

            _cachedPath = Path.Combine(result.Value<string>(), "steamapps", "common", "Deadlock");
            
            if (storage.Cached != null) 
                storage.Cached.Path = _cachedPath;

            return _cachedPath;
        }
        catch
        {
            // Пытаемся получить путь к папке напрямую, через пользователя
            MessageBox.Show(
                "Не удалось получить путь к папке с игрой!\nПожалуйста, укажите его самостоятельно...",
                AppLocalization.MessageBoxInfoTitle, 
                MessageBoxButton.OK, 
                MessageBoxImage.Information
            );

            var pathFromFolder = OpenFolder();
            if (pathFromFolder is null)
            {
                MessageBox.Show(
                    "Не удалось получить путь к папке с игрой!\nПриложение будет закрыто...",
                    AppLocalization.MessageBoxInfoTitle, 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information
                );
                System.Windows.Application.Current.Shutdown();
            }
            
            _cachedPath = pathFromFolder;
            return _cachedPath;
        }
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
        if (CheckExecutable(processPath, storage.Cached?.SteamExecutable))
            return processPath;

        return null;
    }

    private string? OpenFolder()
    {
        do
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Укажите путь к папке с Deadlock",
                Multiselect = false
            };
            if (dialog.ShowDialog() == true && CheckExecutable(dialog.FolderName, storage.Cached?.DeadlockExecutable))
                return dialog.FolderName;
            
            if (MessageBox.Show("Путь указан неверно!\nХотите повторить?", AppLocalization.MessageBoxQuestionTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) 
                break;
        } while (true);

        return null;
    }
}