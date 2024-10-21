namespace TiDeadlock.Entities.Storage;

public record StorageEntity
{
    public string? Path { get; set; }
    public bool IsServiceInstalled { get; set; }
}