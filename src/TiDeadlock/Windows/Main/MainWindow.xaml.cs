using System.Windows;
using System.Windows.Input;
using TiDeadlock.ViewModels.Main;

namespace TiDeadlock.Windows.Main;

public partial class MainWindow
{
    public MainWindow(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
    
    private void MainWindow_OnLoaded(object? sender, EventArgs e)
    {
        // ((MainViewModel)DataContext).OnLoaded();
    }
    
    private void Border_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }
    
    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }
    
    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}