using CommunityToolkit.Mvvm.Input;
using DependencyPropertyGenerator;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Windows.UI;
using WinUIEx;

namespace Riverside.Toolkit.Controls.TitleBar
{
    [DependencyProperty<bool>("IsAutoDragRegionEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsAccentTitleBarEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsMinimizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsMaximizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsClosable", DefaultValue = true)]
    [DependencyProperty<bool>("UseWinUIEverywhere", DefaultValue = false)]
    [DependencyProperty<bool>("MemorizeWindowPosition", DefaultValue = false)]
    [DependencyProperty<Color>("CaptionForegroundInteract")]
    [DependencyProperty<SolidColorBrush>("CurrentForeground")]
    [DependencyProperty<string>("Title", DefaultValue = "Window Title")]
    [DependencyProperty<string>("Subtitle")]
    [DependencyProperty<string>("WindowTag", DefaultValue = "Main")]
    [DependencyProperty<bool>("CanMaximize", IsReadOnly = true)]
    [DependencyProperty<bool>("CanRestore", IsReadOnly = true)]
    [DependencyProperty<bool>("CanMove", IsReadOnly = true)]
    [DependencyProperty<bool>("CanSize", IsReadOnly = true)]

    public partial class TitleBarEx
    {
        // Constants for PostMessage and SetWindowPos
        public const uint WM_SYSCOMMAND = 0x0112;   // System command message
        public const uint SC_MOVE = 0xF010;         // System command for moving window
        public const uint SC_SIZE = 0xF000;         // System command for resizing window

        public event EventHandler<RoutedEventArgs> MaximizeClick;

        public event EventHandler<RoutedEventArgs> SizeClick;

        public event EventHandler<RoutedEventArgs> MoveClick;

        public event EventHandler<RoutedEventArgs> MinimizeClick;

        public event EventHandler<RoutedEventArgs> CloseClick;

        public event EventHandler<RoutedEventArgs> RestoreClick;

        private void MaximizeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindow.Maximize();
            MaximizeClick?.Invoke(sender, e);
        }

        private void SizeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Get the native window handle (HWND)
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(CurrentWindow);

            // Post WM_SYSCOMMAND with SC_SIZE to simulate the size operation
            PostMessage(hwnd, WM_SYSCOMMAND, (IntPtr)SC_SIZE, IntPtr.Zero);
            SizeClick?.Invoke(sender, e);
        }

        private void MoveContextMenu_Click(object sender, RoutedEventArgs e)
        {
            // Get the native window handle (HWND)
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(CurrentWindow);

            // Post WM_SYSCOMMAND with SC_SIZE to simulate the size operation
            PostMessage(hwnd, WM_SYSCOMMAND, (IntPtr)SC_MOVE, IntPtr.Zero);
            MoveClick?.Invoke(sender, e);
        }

        private void MinimizeContextMenu_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindow.Minimize();
            MinimizeClick?.Invoke(sender, e);
        }

        private void CloseContextMenu_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindow.Close();
            CloseClick?.Invoke(sender, e);
        }

        private void RestoreContextMenu_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindow.Restore();
            RestoreClick?.Invoke(sender, e);
        }
    }
}