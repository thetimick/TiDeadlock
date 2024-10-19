using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TiDeadlock.Entities.Storage;

namespace TiDeadlock.Services.Storage;

public interface IStorageService
{
    StorageEntity? Cached { get; }
    bool IsCached { get; }
    
    Task<StorageEntity> ObtainAsync();
    Task SaveAsync();
}

public class StorageService: IStorageService
{
    private const string StorageFilename = "TiDeadlock.storage.json";

    public StorageEntity? Cached { get; private set; }
    
    public bool IsCached => Cached != null;

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true }; 
    
    public async Task<StorageEntity> ObtainAsync()
    {
        if (IsCached)
            return Cached!;
        
        try
        {
            var str = await File.ReadAllTextAsync(StorageFilename);
            Cached = JsonSerializer.Deserialize<StorageEntity>(str);
            return Cached ?? new StorageEntity();
        }
        catch
        {
            Cached = new StorageEntity();
            return Cached;
        }
    }

    public async Task SaveAsync()
    {
        if (IsCached)
            await File.WriteAllTextAsync(StorageFilename, JsonSerializer.Serialize(Cached, _options));
    }
}