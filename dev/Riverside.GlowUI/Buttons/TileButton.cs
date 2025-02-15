﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace CubeKit.UI.Controls.Buttons;

public partial class TileButton : Button
{
    protected Border AnimatingBorder
    {
        get { return (Border)GetTemplateChild("AnimatingBorder"); }
    }

    protected UIElement Root
    {
        get { return (UIElement)GetTemplateChild("RootGrid"); }
    }

    public TileButton()
    {
        this.DefaultStyleKey = typeof(TileButton);
        this.Loaded += TileButton_Loaded;
    }

    public void StartPulse() => VisualStateManager.GoToState(this, "PointerOver", true);

    public void StopPulse() => VisualStateManager.GoToState(this, "Normal", true);

    private async void TileButton_Loaded(object sender, RoutedEventArgs e)
    {
        /*   AnimatingBorder.BorderBrush = (IsEnabled ? Application.Current.Resources["AccentRadialGradientBrush"] : Application.Current.Resources["RedRadialGradientBrush"]) as Brush;
           VisualStateManager.GoToState(this, "PointerOver", true);
           await Task.Delay(500);
           VisualStateManager.GoToState(this, "Normal", true);*/
    }
}
