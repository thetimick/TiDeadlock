using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TiDeadlock.Resources;
using TiDeadlock.Services;
using TiDeadlock.Services.Localization;

namespace TiDeadlock.ViewModels.Main;

public partial class MainViewModel(ILogger<MainViewModel> logger, ILocalizationService localizationService): ViewModelBase
{
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnResetButtonCommand))]
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    private bool _useEnglishForHeroesIsEnabled;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    private bool _useEnglishForHeroes;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnResetButtonCommand))]
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    private bool _useEnglishForItemsIsEnabled;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    private bool _useEnglishForItems;
    
    public void OnLoaded()
    {
        logger.LogInformation("[OnLoaded] Start");
        Prepare();
        logger.LogInformation("[OnLoaded] Finish");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        
        logger.LogInformation(
            "[OnPropertyChanged] EventArgs: {PropertyName}, Value = {PropertyValue}", 
            e.PropertyName, 
            GetType().GetProperty(e.PropertyName ?? string.Empty)?.GetValue(this)
        );
    }

    [RelayCommand(CanExecute = nameof(CanExecuteResetButton))]
    private void TapOnResetButton()
    {
        logger.LogInformation("[TapOnResetButton] Start");
        
        if (!UseEnglishForHeroesIsEnabled)
            localizationService.ChangeLocalizationForHeroes(LocalizationService.Localization.Russian);
        if (!UseEnglishForItemsIsEnabled)
            localizationService.ChangeLocalizationForItems(LocalizationService.Localization.Russian);
        
        Prepare();
        
        MessageBox.Show(
            AppLocalization.MessageBoxDescriptionRestore, 
            AppLocalization.MessageBoxInfoTitle, 
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
        
        logger.LogInformation("[TapOnResetButton] Finish");
    }
    
    [RelayCommand(CanExecute = nameof(CanExecutePatchButton))]
    private void TapOnPatchButton()
    {
        logger.LogInformation("[TapOnPatchButton] Start");
        
        if (UseEnglishForHeroes && UseEnglishForHeroesIsEnabled) 
            localizationService.ChangeLocalizationForHeroes(LocalizationService.Localization.English);
        if (UseEnglishForItems && UseEnglishForItemsIsEnabled) 
            localizationService.ChangeLocalizationForItems(LocalizationService.Localization.English);
        
        Prepare();
        
        MessageBox.Show(
            AppLocalization.MessageBoxDescriptionPatch, 
            AppLocalization.MessageBoxInfoTitle, 
            MessageBoxButton.OK, 
            MessageBoxImage.Information
        );
        
        logger.LogInformation("[apOnPatchButton] Finish");
    }
    
    private void Prepare()
    {
        logger.LogInformation("[Prepare] Start");
        
        var currentLocalizationForHeroes = localizationService.ObtainCurrentLocalizationForHeroes();
        UseEnglishForHeroesIsEnabled = currentLocalizationForHeroes == LocalizationService.Localization.Russian;
        UseEnglishForHeroes = !UseEnglishForHeroesIsEnabled;
        
        var currentLocalizationForItems = localizationService.ObtainCurrentLocalizationForItems();
        UseEnglishForItemsIsEnabled = currentLocalizationForItems == LocalizationService.Localization.Russian;
        UseEnglishForItems = !UseEnglishForItemsIsEnabled;
        
        logger.LogInformation("[Prepare] Finish");
    }

    private bool CanExecuteResetButton()
    {
        return !UseEnglishForHeroesIsEnabled || !UseEnglishForItemsIsEnabled;
    }

    private bool CanExecutePatchButton()
    {
        return (UseEnglishForHeroes && UseEnglishForHeroesIsEnabled) || (UseEnglishForItems && UseEnglishForItemsIsEnabled);
    }
}