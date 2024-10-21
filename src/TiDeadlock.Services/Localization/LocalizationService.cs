using Gameloop.Vdf;
using Gameloop.Vdf.Linq;
using TiDeadlock.Entities.Config;
using TiDeadlock.Services.Config;
using TiDeadlock.Services.Search;

namespace TiDeadlock.Services.Localization;

public interface ILocalizationService
{
    Task<LocalizationService.Localization> ObtainLocalizationForHeroesAsync();
    Task<LocalizationService.Localization> ObtainLocalizationForItemsAsync();
    
    Task ChangeLocalizationForHeroesAsync(LocalizationService.Localization localization = LocalizationService.Localization.English);
    Task ChangeLocalizationForItemsAsync(LocalizationService.Localization localization = LocalizationService.Localization.English);

    Task RestoreAsync();
}

public class LocalizationService(
    IConfigService configService,
    ISearchService searchService
): ILocalizationService {
    public enum Localization
    {
        English,
        Russian,
        Unknown
    }
    
    public async Task<Localization> ObtainLocalizationForHeroesAsync()
    {
        var config = await configService.ObtainAsync();
        return await ObtainLocalizationAsync(config.Localization.CheckingHeroes);
    }

    public async Task<Localization> ObtainLocalizationForItemsAsync()
    {
        var config = await configService.ObtainAsync();
        return await ObtainLocalizationAsync(config.Localization.CheckingItems);
    }

    public async Task ChangeLocalizationForHeroesAsync(Localization localization)
    {
        var config = await configService.ObtainAsync();
        
        var currentLocalization = await ObtainLocalizationAsync(config.Localization.CheckingHeroes);
        if (currentLocalization == localization)
            return;
        
        await ChangeLocalizationAsync(localization, config.Localization.HeroPrefix);
    }

    public async Task ChangeLocalizationForItemsAsync(Localization localization)
    {
        var config = await configService.ObtainAsync();
        
        var currentLocalization = await ObtainLocalizationAsync(config.Localization.CheckingItems);
        if (currentLocalization == localization)
            return;
        
        await ChangeLocalizationAsync(localization, config.Localization.ItemPrefix);
    }

    public async Task RestoreAsync()
    {
        var config = await configService.ObtainAsync();
        var path = await searchService.ObtainAsync();
        
        if (path == null)
            throw new Exception("Не удалось получить путь к папке с игрой!");
            
        RestoreBackup(path, config);
    }

    private async Task<Localization> ObtainLocalizationAsync(ConfigEntity.LocalizationEntity.CheckingEntity checkingEntity)
    {
        var config = await configService.ObtainAsync();
        var path = await searchService.ObtainAsync();

        if (path == null)
            throw new Exception("Не удалось получить путь к папке с игрой!");
        
        var vdfString = await File.ReadAllTextAsync(Path.Combine(path, config.Localization.RussianFileName));
        var vdfDeserialized = VdfConvert.Deserialize(vdfString);
        if (vdfDeserialized.Value[config.Localization.MainKey]?[checkingEntity.Key] is not VValue { Value: string value })
            throw new Exception($"Ошибка при чтении файла {Path.GetFileName(config.Localization.RussianFileName)}");
        
        if (value == checkingEntity.EnglishValue)
            return Localization.English;
        
        return value == checkingEntity.RussianValue 
            ? Localization.Russian 
            : Localization.Unknown;
    }

    private async Task ChangeLocalizationAsync(Localization localization, string prefix)
    {
        var config = await configService.ObtainAsync();
        var path = await searchService.ObtainAsync();
        
        if (path == null)
            throw new Exception("Не удалось получить путь к папке с игрой!");
            
        if (localization == Localization.Russian)
        {
            RestoreBackup(path, config);
            return;
        }
        
        var englishLocalizationFileName = Path.Combine(path, config.Localization.EnglishFileName);
        var russianLocalizationFileName = Path.Combine(path, config.Localization.RussianFileName);
        var backupLocalizationFileName = Path.Combine(path, config.Localization.RussianFileName + config.Localization.BackupExtension);
        
        var vdfEnglish = await File.ReadAllTextAsync(englishLocalizationFileName);
        var vdfRussian = await File.ReadAllTextAsync(russianLocalizationFileName);

        var vdfDeserializedEnglish = VdfConvert.Deserialize(vdfEnglish);
        var vdfDeserializedRussian = VdfConvert.Deserialize(vdfRussian);

        var tokensForEnglish = vdfDeserializedEnglish.Value[config.Localization.MainKey]
            ?.Where(token => (token as VProperty)?.Key.StartsWith(prefix) ?? false);
        
        var russianTokens = vdfDeserializedRussian.Value[config.Localization.MainKey];

        if (tokensForEnglish == null)
            throw new Exception($"Ошибка при чтении файла {Path.GetFileName(config.Localization.EnglishFileName)}");
        if (russianTokens == null)
            throw new Exception($"Ошибка при чтении файла {Path.GetFileName(config.Localization.RussianFileName)}");
                
        foreach (var token in tokensForEnglish)
        {
            if (token is not VProperty property)
                continue;
            russianTokens[property.Key] = new VValue(property.Value);
        }

        vdfDeserializedRussian.Value[config.Localization.MainKey] = russianTokens;

        if (File.Exists(backupLocalizationFileName))
        {
            await File.WriteAllTextAsync(russianLocalizationFileName, VdfConvert.Serialize(vdfDeserializedRussian));
            return;
        }
        
        File.Copy(russianLocalizationFileName, backupLocalizationFileName);
        await File.WriteAllTextAsync(russianLocalizationFileName, VdfConvert.Serialize(vdfDeserializedRussian));
    }

    private static void RestoreBackup(string path, ConfigEntity config)
    {
        var russianLocalizationFileName = Path.Combine(path, config.Localization.RussianFileName);
        var backupLocalizationFileName = Path.Combine(path, config.Localization.RussianFileName + config.Localization.BackupExtension);

        if (!File.Exists(backupLocalizationFileName))
            throw new Exception("Отсутствует Backup файл! Пожалуйста, сделайте полную проверку файлов в Steam для восстановления локализации...");
        
        if (File.Exists(russianLocalizationFileName))
            File.Delete(russianLocalizationFileName);
        File.Copy(backupLocalizationFileName, russianLocalizationFileName);
        File.Delete(backupLocalizationFileName);
    }
}