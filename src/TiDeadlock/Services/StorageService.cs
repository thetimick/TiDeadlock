using System.IO;
using System.Text.Json;
using TiDeadlock.Entities;

namespace TiDeadlock.Services;

public interface IStorageService
{
    StorageEntity? Entity { get; }
    
    Task<StorageEntity> ObtainAsync();
    Task SaveAsync();
}

public class StorageService: IStorageService
{
    private const string StorageFilename = "TiDeadlock.storage.json";
    
    public StorageEntity? Entity { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true }; 
    
    public async Task<StorageEntity> ObtainAsync()
    {
        if (Entity != null)
            return Entity;
        
        try
        {
            var str = await File.ReadAllTextAsync(StorageFilename);
            Entity = JsonSerializer.Deserialize<StorageEntity>(str);
            return Entity ?? new StorageEntity();
        }
        catch
        {
            Entity = new StorageEntity();
            return Entity;
        }
    }

    public async Task SaveAsync()
    {
        if (Entity != null)
            await File.WriteAllTextAsync(StorageFilename, JsonSerializer.Serialize(Entity, _options));
    }
}