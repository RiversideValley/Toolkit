﻿#if WinUI
using DependencyPropertyGenerator;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using WinUIEx;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar;

// Available properties
[DependencyProperty<string>("Title", DefaultValue = "Window Title")]
[DependencyProperty<string>("Subtitle")]
[DependencyProperty<string>("WindowTag", DefaultValue = "Main")]
[DependencyProperty<bool>("IsAutoDragRegionEnabled", DefaultValue = true, OnChanged = "OnTitleBarPropertyChanged")]
[DependencyProperty<bool>("IsAccentTitleBarEnabled", DefaultValue = true, OnChanged = "OnTitleBarPropertyChanged")]
[DependencyProperty<bool>("IsMinimizable", DefaultValue = true, OnChanged = "OnTitleBarPropertyChanged")]
[DependencyProperty<bool>("IsMaximizable", DefaultValue = true, OnChanged = "OnTitleBarPropertyChanged")]
[DependencyProperty<bool>("IsClosable", DefaultValue = true, OnChanged = "OnTitleBarPropertyChanged")]
[DependencyProperty<bool>("UseWinUIEverywhere", DefaultValue = false, OnChanged = "OnTitleBarPropertyChanged")]
[DependencyProperty<bool>("MemorizeWindowPosition", DefaultValue = false, OnChanged = "OnTitleBarPropertyChanged")]

// Required for style
[DependencyProperty<Color>("CaptionForegroundInteract")]
[DependencyProperty<SolidColorBrush>("CurrentForeground")]
[DependencyProperty<string>("CloseButtonRegularStyleKey", DefaultValue = "Close")]
[DependencyProperty<string>("CloseButtonSingularStyleKey", DefaultValue = "CloseSingular")]

// Required for WinUI system menu
[DependencyProperty<bool>("CanMaximize", IsReadOnly = true)]
[DependencyProperty<bool>("CanRestore", IsReadOnly = true)]
[DependencyProperty<bool>("CanMove", IsReadOnly = true)]
[DependencyProperty<bool>("CanSize", IsReadOnly = true)]

public partial class TitleBarEx
{
    // Event handlers for menu actions
    public event EventHandler<RoutedEventArgs>? MaximizeClick;
    public event EventHandler<RoutedEventArgs>? SizeClick;
    public event EventHandler<RoutedEventArgs>? MoveClick;
    public event EventHandler<RoutedEventArgs>? MinimizeClick;
    public event EventHandler<RoutedEventArgs>? CloseClick;
    public event EventHandler<RoutedEventArgs>? RestoreClick;

    private void OnTitleBarPropertyChanged(bool oldValue, bool newValue) => InvokeChecks();

    private void MaximizeContextMenu_Click(object sender, RoutedEventArgs e)
    {
        // Maximize the window
        this.CurrentWindow?.Maximize();
        MaximizeClick?.Invoke(sender, e);
    }

    private void SizeContextMenu_Click(object sender, RoutedEventArgs e)
    {
        // Invoke resize
        this.CurrentWindow?.InvokeResize();
        SizeClick?.Invoke(sender, e);
    }

    private void MoveContextMenu_Click(object sender, RoutedEventArgs e)
    {
        // Invoke move
        this.CurrentWindow?.InvokeMove();
        MoveClick?.Invoke(sender, e);
    }

    private void MinimizeContextMenu_Click(object sender, RoutedEventArgs e)
    {
        // Minimize the window
        this.CurrentWindow?.Minimize();
        MinimizeClick?.Invoke(sender, e);
    }

    private void CloseContextMenu_Click(object sender, RoutedEventArgs e)
    {
        // Close the window
        this.CurrentWindow?.Close();
        CloseClick?.Invoke(sender, e);
    }

    private void RestoreContextMenu_Click(object sender, RoutedEventArgs e)
    {
        // Restore the window
        this.CurrentWindow?.Restore();
        RestoreClick?.Invoke(sender, e);
    }
}
#endif