namespace TiDeadlock.Entities.Storage;

public record StorageEntity
{
    public string? Path { get; set; }
    public string DeadlockExecutable { get; init; } = @"game\bin\win64\project8.exe";
    public string SteamExecutable { get; init; } = "steam.exe";
}