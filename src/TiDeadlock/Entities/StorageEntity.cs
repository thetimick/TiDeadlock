namespace TiDeadlock.Entities;

public record StorageEntity
{
    public string? Path { get; set; }
    public string DeadlockExecutable { get; set; } = @"game\bin\win64\project8.exe";
    public string SteamExecutable { get; set; } = "steam.exe";
}