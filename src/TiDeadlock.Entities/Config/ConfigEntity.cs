namespace TiDeadlock.Entities.Config;

public record ConfigEntity
{
    public record LocalizationEntity
    {
        public record CheckingEntity
        {
            public string? Key { get; init; }
            public string? EnglishValue { get; init; }
            public string? RussianValue { get; init; }
        }
        
        public string? EnglishFileName { get; init; } = 
            @"game\citadel\resource\localization\citadel_gc\citadel_gc_english.txt";
        public string? RussianFileName { get; init; } =
            @"game\citadel\resource\localization\citadel_gc\citadel_gc_russian.txt";

        public string? BackupExtension { get; init; } = ".bak";

        public string? MainKey { get; init; } = "Tokens";

        public string? HeroPrefix { get; init; } = "hero_";
        public string? ItemPrefix { get; init; } = "upgrade_";

        public CheckingEntity? CheckingHeroes { get; init; } = new()
        {
            Key = "hero_mirage",
            EnglishValue = "Mirage",
            RussianValue = "Мираж"
        };
        
        public CheckingEntity? CheckingItems { get; init; } = new()
        {
            Key = "upgrade_damage_recycler",
            EnglishValue = "Leech",
            RussianValue = "Кровопийца"
        };
    }

    public string? SteamExecutable { get; init; } = "steam.exe";
    public string? DeadlockExecutable { get; init; } = @"game\bin\win64\project8.exe";

    public LocalizationEntity? Localization { get; init; } = new();
}