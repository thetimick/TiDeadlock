using System.Text.Json;
using TiDeadlock.Entities.Config;

namespace TiDeadlock.Services.Config;

public interface IConfigService
{
    ConfigEntity? Cached { get; }
    bool IsCached { get; }
    
    Task<ConfigEntity> ObtainAsync();
}

public class ConfigService: IConfigService
{
    private const string Url = 
        "https://raw.githubusercontent.com/thetimick/PublicStorage/refs/heads/main/TD/Configs/config.json";
    
    public ConfigEntity? Cached { get; private set; }
    public bool IsCached => Cached != null;
    
    public async Task<ConfigEntity> ObtainAsync()
    {
        if (IsCached)
            return Cached!;
        
        Cached = await ObtainConfigAsync();
        return Cached;
    }
    
    private static async Task<ConfigEntity> ObtainConfigAsync()
    {
        using var client = new HttpClient();
        
        var response = await client.GetAsync(Url);
        if (!response.IsSuccessStatusCode)
            return new ConfigEntity();
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ConfigEntity>(content) ?? new ConfigEntity();
    }
}