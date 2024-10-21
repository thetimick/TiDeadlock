using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TiDeadlock.ViewModels.Main;

namespace TiDeadlock.Windows.Main;

public partial class MainWindow
{
    private readonly ILogger<MainWindow> _logger;
    
    public MainWindow(ILogger<MainWindow> logger, MainViewModel viewModel)
    {
        _logger = logger;
        DataContext = viewModel;
        
        InitializeComponent();
    }
    
    private void TitleBarOnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }
    
    private void MinimizeButtonClick(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("[MinimizeButtonClick]");
        WindowState = WindowState.Minimized;
    }
    
    private void CloseButtonClick(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("[CloseButtonClick]");
        Close();
    }
}