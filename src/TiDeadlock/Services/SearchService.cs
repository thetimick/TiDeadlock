using System.Diagnostics;
using System.IO;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

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
        if (_cachedPath is not null)
            return _cachedPath;

        var pathFromStorage = storage.Obtain().Path;
        if (pathFromStorage is not null && File.Exists(pathFromStorage))
        {
            _cachedPath = pathFromStorage;
            return _cachedPath;
        }
        
        try
        {
            var steamProcess = Process.GetProcesses().FirstOrDefault(process => process.ProcessName == "steam");
            var steamDirectory = Path.GetDirectoryName(steamProcess?.MainModule?.FileName);
            ArgumentException.ThrowIfNullOrEmpty(steamDirectory);
            
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
            
            if (storage.Entity != null) 
                storage.Entity.Path = _cachedPath;

            return _cachedPath;
        }
        catch
        {
            return null;
        }
    }
}