namespace TiDeadlock.Entities.Config;

public record ConfigEntity
{
    public record LocalizationEntity
    {
        public string? EnglishFileName { get; init; }
        public string? RussianFileName { get; init; }
        
        public string? BackupExtension { get; init; }

        public string? MainKey { get; init; }

        public string? HeroPrefix { get; init; }
        public string? ItemPrefix { get; init; } 
    }
    
    public string? SteamExecutable { get; init; }
    public string? DeadlockExecutable { get; init; }
    
    public LocalizationEntity? Localization { get; init; }
}