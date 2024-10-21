using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TiDeadlock.Resources;
using TiDeadlock.Services.Localization;
using TiDeadlock.Services.RunLoop;
using TiDeadlock.Windows.Settings;

namespace TiDeadlock.ViewModels.Main;

public partial class MainViewModel(
    ILogger<MainViewModel> logger, 
    ILocalizationService localizationService,
    IRunLoopService runLoopService
 ): ObservableObject {
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnResetButtonCommand))]
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    private bool _useEnglishForHeroesIsEnabled;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    [NotifyCanExecuteChangedFor(nameof(TapOnServiceButtonCommand))]
    private bool _useEnglishForHeroes;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnResetButtonCommand))]
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    private bool _useEnglishForItemsIsEnabled;
    
    [ObservableProperty] 
    [NotifyCanExecuteChangedFor(nameof(TapOnPatchButtonCommand))]
    [NotifyCanExecuteChangedFor(nameof(TapOnServiceButtonCommand))]
    private bool _useEnglishForItems;
    
    public async Task OnLoaded()
    {
        logger.LogInformation("[OnLoaded] Start");
        await Prepare();
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
    private async Task TapOnResetButton()
    {
        logger.LogInformation("[TapOnResetButton] Start");

        await localizationService.RestoreAsync();
        
        await Prepare();
        
        MessageBox.Show(
            AppLocalization.MessageBoxDescriptionRestore, 
            AppLocalization.MessageBoxInfoTitle, 
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );
        
        logger.LogInformation("[TapOnResetButton] Finish");
    }
    
    [RelayCommand(CanExecute = nameof(CanExecutePatchButton))]
    private async Task TapOnPatchButton()
    {
        logger.LogInformation("[TapOnPatchButton] Start");
        
        if (UseEnglishForHeroes && UseEnglishForHeroesIsEnabled) 
            await localizationService.ChangeLocalizationForHeroesAsync();
        if (UseEnglishForItems && UseEnglishForItemsIsEnabled) 
            await localizationService.ChangeLocalizationForItemsAsync();
        
        await Prepare();
        
        MessageBox.Show(
            AppLocalization.MessageBoxDescriptionPatch, 
            AppLocalization.MessageBoxInfoTitle, 
            MessageBoxButton.OK, 
            MessageBoxImage.Information
        );
        
        logger.LogInformation("[apOnPatchButton] Finish");
    }

    [RelayCommand(CanExecute = nameof(CanExecuteServiceButton))]
    private async Task TapOnServiceButton()
    {
        var isInstalled = await runLoopService.IsInstalledAsync();
        if (isInstalled)
        {
            await runLoopService.UninstallAsync();
            MessageBox.Show(
                "Сервисный режим был отключен!", 
                AppLocalization.MessageBoxServiceTitle,
                MessageBoxButton.OK, 
                MessageBoxImage.Information
            );
            return;
        }
        
        var mbResult = MessageBox.Show(
            AppLocalization.MessageBoxServiceDescription,
            AppLocalization.MessageBoxServiceTitle, 
            MessageBoxButton.YesNo, 
            MessageBoxImage.Question
        );
        
        if (mbResult != MessageBoxResult.Yes)
            return;
        
        await runLoopService.InstallAsync(UseEnglishForHeroes, UseEnglishForItems);
    }
}

#region Private Methods

public partial class MainViewModel
{
    private async Task Prepare()
    {
        logger.LogInformation("[Prepare] Start");
        
        var currentLocalizationForHeroes = await localizationService.ObtainLocalizationForHeroesAsync();
        UseEnglishForHeroesIsEnabled = currentLocalizationForHeroes == LocalizationService.Localization.Russian;
        UseEnglishForHeroes = !UseEnglishForHeroesIsEnabled;
        
        var currentLocalizationForItems = await localizationService.ObtainLocalizationForItemsAsync();
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
    
    private bool CanExecuteServiceButton()
    {
        return UseEnglishForHeroes || UseEnglishForHeroesIsEnabled;
    }
}

#endregion