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
    
    private void Border_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }
    
    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("Minimizing...");
        WindowState = WindowState.Minimized;
    }
    
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        _logger.LogInformation("Closing...");
        Close();
    }
}