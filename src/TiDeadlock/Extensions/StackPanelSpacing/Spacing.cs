﻿using System.Windows;
using JetBrains.Annotations;

namespace TiDeadlock.Extensions.StackPanelSpacing;

public class Spacing
{
    public static readonly DependencyProperty VerticalProperty =
        DependencyProperty.RegisterAttached(
            "Vertical", 
            typeof (double),
            typeof (Spacing),
            new UIPropertyMetadata(0d, VerticalChangedCallback)
        );

    public static readonly DependencyProperty HorizontalProperty =
        DependencyProperty.RegisterAttached(
            "Horizontal", 
            typeof (double), 
            typeof (Spacing),
            new UIPropertyMetadata(0d, HorizontalChangedCallback)
        );
    
    public static double GetHorizontal(DependencyObject obj)
    {
        return (double) obj.GetValue(HorizontalProperty);
    }

    public static double GetVertical(DependencyObject obj)
    {
        return (double) obj.GetValue(VerticalProperty);
    }

    private static void HorizontalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        var space = (double) e.NewValue;
        var obj = (DependencyObject) sender;

        MarginSetter.SetMargin(obj, new Thickness(0, 0, space, 0));
        MarginSetter.SetLastItemMargin(obj, new Thickness(0));
    }

    [UsedImplicitly]
    public static void SetHorizontal(DependencyObject obj, double space)
    {
        obj.SetValue(HorizontalProperty, space);
    }

    [UsedImplicitly]
    public static void SetVertical(DependencyObject obj, double value)
    {
        obj.SetValue(VerticalProperty, value);
    }

    private static void VerticalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        var space = (double) e.NewValue;
        var obj = (DependencyObject) sender;
        MarginSetter.SetMargin(obj, new Thickness(0, 0, 0, space));
        MarginSetter.SetLastItemMargin(obj, new Thickness(0));
    }
}