using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace TiDeadlock.Extensions.StackPanelSpacing;

public class MarginSetter
{
    public static readonly DependencyProperty MarginProperty = 
        DependencyProperty.RegisterAttached(
            "Margin", 
            typeof (Thickness), 
            typeof (MarginSetter), 
            new UIPropertyMetadata(new Thickness(), MarginChangedCallback)
        );
    
    public static readonly DependencyProperty LastItemMarginProperty = 
        DependencyProperty.RegisterAttached(
            "LastItemMargin", 
            typeof (Thickness), 
            typeof (MarginSetter), 
            new UIPropertyMetadata(new Thickness(), MarginChangedCallback)
        );
    
    private static Thickness GetLastItemMargin(DependencyObject obj)
    {
        return (Thickness) obj.GetValue(LastItemMarginProperty);
    }

    [UsedImplicitly]
    public static Thickness GetMargin(DependencyObject obj)
    {
        return (Thickness) obj.GetValue(MarginProperty);
    }

    private static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not Panel panel) 
            return;

        panel.Loaded -= OnPanelLoaded;
        panel.Loaded += OnPanelLoaded;

        if (panel.IsLoaded)
            OnPanelLoaded(panel, null);
    }

    private static void OnPanelLoaded(object sender, RoutedEventArgs? e)
    {
        var panel = (Panel) sender;

        for (var i = 0; i < panel.Children.Count; i++)
        {
            if (panel.Children[i] is not FrameworkElement fe)
                continue;
            
            var isLastItem = i == panel.Children.Count - 1;
            fe.Margin = isLastItem ? GetLastItemMargin(panel) : GetMargin(panel);
        }
    }

    [UsedImplicitly]
    public static void SetLastItemMargin(DependencyObject obj, Thickness value)
    {
        obj.SetValue(LastItemMarginProperty, value);
    }

    [UsedImplicitly]
    public static void SetMargin(DependencyObject obj, Thickness value)
    {
        obj.SetValue(MarginProperty, value);
    }
}