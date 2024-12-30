using DependencyPropertyGenerator;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;
using WinUIEx;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
    // Available properties
    [DependencyProperty<string>("Title", DefaultValue = "Window Title")]
    [DependencyProperty<string>("Subtitle")]
    [DependencyProperty<string>("WindowTag", DefaultValue = "Main")]
    [DependencyProperty<bool>("IsAutoDragRegionEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsAccentTitleBarEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsMinimizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsMaximizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsClosable", DefaultValue = true)]
    [DependencyProperty<bool>("UseWinUIEverywhere", DefaultValue = false)]
    [DependencyProperty<bool>("MemorizeWindowPosition", DefaultValue = false)]

    // Required for style
    [DependencyProperty<Color>("CaptionForegroundInteract")]
    [DependencyProperty<SolidColorBrush>("CurrentForeground")]

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

        private void MaximizeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Maximize the window
            CurrentWindow?.Maximize();
            MaximizeClick?.Invoke(sender, e);
        }

        private void SizeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Invoke resize
            CurrentWindow?.InvokeResize();
            SizeClick?.Invoke(sender, e);
        }

        private void MoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Invoke move
            CurrentWindow?.InvokeMove();
            MoveClick?.Invoke(sender, e);
        }

        private void MinimizeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Minimize the window
            CurrentWindow?.Minimize();
            MinimizeClick?.Invoke(sender, e);
        }

        private void CloseContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            CurrentWindow?.Close();
            CloseClick?.Invoke(sender, e);
        }

        private void RestoreContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Restore the window
            CurrentWindow?.Restore();
            RestoreClick?.Invoke(sender, e);
        }
    }
}