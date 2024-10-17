namespace TiDeadlock.Entities.Update;

public record UpdateEntity
{
    public record VersionEntity
    {
        public double Version { get; init; } = 1.0;
        public string Link { get; init; } = string.Empty;
    }
    
    public VersionEntity CurrentVersion { get; init; } = new();
    public List<VersionEntity> OldVersions { get; init; } = [];
}