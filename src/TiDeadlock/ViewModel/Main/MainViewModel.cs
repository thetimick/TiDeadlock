using System.Windows;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TiDeadlock.Application;
using TiDeadlock.Resources;
using TiDeadlock.Services;

namespace TiDeadlock.ViewModel.Main;

public partial class MainViewModel: ObservableObject
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
    
    private readonly ILocalizationService _localizationService;
    
    public MainViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        UpdateData();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteResetButton))]
    private void TapOnResetButton()
    {
        if (!UseEnglishForHeroesIsEnabled)
            _localizationService.ChangeLocalizationForHeroes(LocalizationService.Localization.Russian);
        if (!UseEnglishForItemsIsEnabled)
            _localizationService.ChangeLocalizationForItems(LocalizationService.Localization.Russian);
        
        UpdateData();

        MessageBox.Show(AppLocalization.MessageBoxDescriptionRestore, AppLocalization.MessageBoxInfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    [RelayCommand(CanExecute = nameof(CanExecutePatchButton))]
    private void TapOnPatchButton()
    {
        if (UseEnglishForHeroes && UseEnglishForHeroesIsEnabled) 
            _localizationService.ChangeLocalizationForHeroes(LocalizationService.Localization.English);
        if (UseEnglishForItems && UseEnglishForItemsIsEnabled) 
            _localizationService.ChangeLocalizationForItems(LocalizationService.Localization.English);
        
        UpdateData();
        
        MessageBox.Show(AppLocalization.MessageBoxDescriptionPatch, AppLocalization.MessageBoxInfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    private void UpdateData()
    {
        var currentLocalizationForHeroes = _localizationService.ObtainCurrentLocalizationForHeroes();
        UseEnglishForHeroesIsEnabled = currentLocalizationForHeroes == LocalizationService.Localization.Russian;
        UseEnglishForHeroes = !UseEnglishForHeroesIsEnabled;
        
        var currentLocalizationForItems = _localizationService.ObtainCurrentLocalizationForItems();
        UseEnglishForItemsIsEnabled = currentLocalizationForItems == LocalizationService.Localization.Russian;
        UseEnglishForItems = !UseEnglishForItemsIsEnabled;
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