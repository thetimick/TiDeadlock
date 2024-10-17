using System.IO;
using System.Windows;
using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

namespace TiDeadlock.Services;

public interface ILocalizationService
{
    LocalizationService.Localization? ObtainCurrentLocalizationForHeroes();
    LocalizationService.Localization? ObtainCurrentLocalizationForItems();
    
    
    bool ChangeLocalizationForHeroes(LocalizationService.Localization localization);
    bool ChangeLocalizationForItems(LocalizationService.Localization localization);
}

public class LocalizationService(ISearchService search): ILocalizationService
{
    public enum Localization
    {
        English,
        Russian
    }

    private const string English = @"game\citadel\resource\localization\citadel_gc\citadel_gc_english.txt";
    private const string Russian = @"game\citadel\resource\localization\citadel_gc\citadel_gc_russian.txt";
    private const string RussianBackup = @"game\citadel\resource\localization\citadel_gc\citadel_gc_russian.txt.bak";
    
    public Localization? ObtainCurrentLocalizationForHeroes()
    {
        var path = search.GetPathForDeadlock();
        ArgumentException.ThrowIfNullOrEmpty(path);
        
        var russianFile = File.ReadAllText(Path.Combine(path, Russian));
        var russian = VdfConvert.Deserialize(russianFile);
        if (russian.Value["Tokens"]?["hero_mirage"] is not VValue mirage) 
            return null;

        return mirage.Value switch
        {
            "Mirage" => Localization.English,
            "Мираж" => Localization.Russian,
            _ => null
        };
    }

    public Localization? ObtainCurrentLocalizationForItems()
    {
        var path = search.GetPathForDeadlock();
        ArgumentException.ThrowIfNullOrEmpty(path);
        
        var russianFile = File.ReadAllText(Path.Combine(path, Russian));
        var russian = VdfConvert.Deserialize(russianFile);
        if (russian.Value["Tokens"]?["upgrade_damage_recycler"] is not VValue leech) 
            return null;

        return leech.Value switch
        {
            "Leech" => Localization.English,
            "Кровопийца" => Localization.Russian,
            _ => null
        };
    }

    public bool ChangeLocalizationForHeroes(Localization localization)
    {
        if (ObtainCurrentLocalizationForHeroes() == localization)
            return false;
        
        try
        {
            var path = search.GetPathForDeadlock();
            ArgumentException.ThrowIfNullOrEmpty(path);
            
            if (localization == Localization.Russian)
            {
                File.Delete(Path.Combine(path, Russian));
                File.Copy(Path.Combine(path, RussianBackup), Path.Combine(path, Russian));
                File.Delete(Path.Combine(path, RussianBackup));
                return true;
            }
            
            var englishFile = File.ReadAllText(Path.Combine(path, English));
            var russianFile = File.ReadAllText(Path.Combine(path, Russian));

            var english = VdfConvert.Deserialize(englishFile);
            var russian = VdfConvert.Deserialize(russianFile);

            var heroesForEnglish = english.Value["Tokens"]
                ?.Where(token => (token as VProperty)?.Key.StartsWith("hero_") ?? false);

            var heroesForRussian = russian.Value["Tokens"]
                ?.Where(token => (token as VProperty)?.Key.StartsWith("hero_") ?? false);

            var russianTokens = russian.Value["Tokens"];

            ArgumentNullException.ThrowIfNull(heroesForEnglish);
            ArgumentNullException.ThrowIfNull(heroesForRussian);
            ArgumentNullException.ThrowIfNull(russianTokens);

            foreach (var hero in heroesForEnglish)
            {
                if (hero is not VProperty heroProperty)
                    continue;
                russianTokens[heroProperty.Key] = new VValue(heroProperty.Value);
            }

            russian.Value["Tokens"] = russianTokens;

            if (!File.Exists(Path.Combine(path, RussianBackup)))
                File.Copy(Path.Combine(path, Russian), Path.Combine(path, RussianBackup));

            File.WriteAllText(Path.Combine(path, Russian), VdfConvert.Serialize(russian));

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ChangeLocalizationForItems(Localization localization)
    {
        if (ObtainCurrentLocalizationForItems() == localization)
            return false;
        
        try
        {
            var path = search.GetPathForDeadlock();
            ArgumentException.ThrowIfNullOrEmpty(path);
            
            if (localization == Localization.Russian)
            {
                File.Delete(Path.Combine(path, Russian));
                File.Copy(Path.Combine(path, RussianBackup), Path.Combine(path, Russian));
                File.Delete(Path.Combine(path, RussianBackup));
                return true;
            }
            
            var englishFile = File.ReadAllText(Path.Combine(path, English));
            var russianFile = File.ReadAllText(Path.Combine(path, Russian));

            var english = VdfConvert.Deserialize(englishFile);
            var russian = VdfConvert.Deserialize(russianFile);

            var itemsForEnglish = english.Value["Tokens"]
                ?.Where(token => (token as VProperty)?.Key.StartsWith("upgrade_") ?? false);

            var itemsForRussian = russian.Value["Tokens"]
                ?.Where(token => (token as VProperty)?.Key.StartsWith("upgrade_") ?? false);

            var russianTokens = russian.Value["Tokens"];

            ArgumentNullException.ThrowIfNull(itemsForEnglish);
            ArgumentNullException.ThrowIfNull(itemsForRussian);
            ArgumentNullException.ThrowIfNull(russianTokens);

            foreach (var item in itemsForEnglish)
            {
                if (item is not VProperty itemProperty)
                    continue;
                russianTokens[itemProperty.Key] = new VValue(itemProperty.Value);
            }

            russian.Value["Tokens"] = russianTokens;

            if (!File.Exists(Path.Combine(path, RussianBackup)))
                File.Copy(Path.Combine(path, Russian), Path.Combine(path, RussianBackup));

            File.WriteAllText(Path.Combine(path, Russian), VdfConvert.Serialize(russian));

            return true;
        }
        catch
        {
            return false;
        }
    }
}