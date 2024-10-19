using System.Text.Json;
using TiDeadlock.Entities.Config;

namespace TiDeadlock.Services.Config;

public interface IConfigService
{
    ConfigEntity? Cached { get; }
    bool IsCached { get; }
    
    Task<bool> ObtainAsync();
}

public class ConfigService: IConfigService
{
    private const string Url = "https://raw.githubusercontent.com/thetimick/PublicStorage/refs/heads/main/TD/Configs/config.json";
    
    public ConfigEntity? Cached { get; private set; }
    public bool IsCached => Cached != null;
    
    public async Task<bool> ObtainAsync()
    {
        if (IsCached)
            return true;

        var config = await ObtainConfigAsync();
        if (config == null) 
            return false;
        
        Cached = config;
        return true;
    }
    
    private static async Task<ConfigEntity?> ObtainConfigAsync()
    {
        using var client = new HttpClient();
        
        var response = await client.GetAsync(Url);
        if (!response.IsSuccessStatusCode)
            return null;
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ConfigEntity>(content);
    }
}