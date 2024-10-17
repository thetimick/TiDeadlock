using System.IO;
using System.Text.Json;
using TiDeadlock.Entities;

namespace TiDeadlock.Services;

public interface IStorageService
{
    StorageEntity? Entity { get; }
    
    StorageEntity Obtain();
    void Save();
}

public class StorageService: IStorageService
{
    private const string StorageFilename = "TiDeadlock.storage.json";
    
    public StorageEntity? Entity { get; private set; }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true }; 
    
    public StorageEntity Obtain()
    {
        if (Entity != null)
            return Entity;
        
        try
        {
            Entity = JsonSerializer.Deserialize<StorageEntity>(File.ReadAllText(StorageFilename));
            return Entity ?? new StorageEntity();
        }
        catch
        {
            Entity = new StorageEntity();
            return Entity;
        }
    }

    public void Save()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(Entity);
            File.WriteAllText(StorageFilename, JsonSerializer.Serialize(Entity, _options));
        }
        catch
        {
            // ignored
        }
    }
}