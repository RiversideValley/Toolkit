using System.Threading.Tasks;
using static Riverside.Toolkit.Helpers.NativeHelper;
using Windows.Graphics;
using System;
using WinUIEx;
using Riverside.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using System.Diagnostics;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        private async void LoadDragRegion()
        {
            // Make sure the loop doesn't trigger too often
            await Task.Delay(100);

            try
            {
                // If the window has been closed, break the loop
                if (closed) return;

                // Check if every condition is met
                if (CurrentWindow?.AppWindow is not null && IsAutoDragRegionEnabled)
                {
                    // Width (Scaled window width)
                    var width = (int)(CurrentWindow.Bounds.Width * Display.Scale(CurrentWindow));

                    // Height (Scaled control actual height)
                    var height = (int)((ActualHeight + buttonDownHeight) * Display.Scale(CurrentWindow));

                    // Set the drag region for the window's title bar
                    CurrentWindow.AppWindow.TitleBar.SetDragRectangles([new RectInt32(0, 0, width, height)]);
                }

                // Recursive call to keep updating bounds
                LoadDragRegion();
            }
            catch
            {
                // Silent catch to ensure the recursive loop continues
                LoadDragRegion();
                return;
            }
        }

        public void SetWindowIcon(string path)
        {
            try
            {
                // Attempt to set the title bar icon
                if (TitleBarIcon is not null)
                {
                    var uri = new Uri($"ms-appx:///{path}", UriKind.RelativeOrAbsolute);
                    TitleBarIcon.Source = new BitmapImage(uri);
                }
            }
            catch
            {

            }

            try
            {
                // Set the window icon
                string iconPath = Path.Combine(AppContext.BaseDirectory, path);
                CurrentWindow?.SetIcon(iconPath);
            }
            catch
            {

            }
        }

        bool wasMaximized = false;

        private void CheckMaximization()
        {
            if (closed || !allowSizeCheck) return;

            if (CurrentWindow?.Presenter is OverlappedPresenter presenter)
            {
                switch (presenter.State)
                {
                    case OverlappedPresenterState.Maximized:
                        HandleMaximizedState();
                        break;

                    case OverlappedPresenterState.Restored:
                        HandleRestoredState();
                        break;
                }
            }
            else
            {
                HandleUnknownState();
            }

            wasMaximized = isMaximized;

            // Local method to handle the maximized state
            void HandleMaximizedState()
            {
                if (MemorizeWindowPosition) SetValue($"{WindowTag}Maximized", true);

                additionalHeight = WND_FRAME_TOP_MAXIMIZED; // Required for window drag region
                isMaximized = true; // Required for NCHITTEST

                if (wasMaximized != isMaximized) SwitchState(ButtonsState.None);
            }

            // Local method to handle the restored state
            void HandleRestoredState()
            {
                if (MemorizeWindowPosition)
                {
                    SetValue($"{WindowTag}Maximized", false);
                    SetValue<double>($"{WindowTag}PositionX", CurrentWindow.AppWindow.Position.X);
                    SetValue<double>($"{WindowTag}PositionY", CurrentWindow.AppWindow.Position.Y);
                    SetValue<double>($"{WindowTag}Width", CurrentWindow.AppWindow.Size.Width);
                    SetValue<double>($"{WindowTag}Height", CurrentWindow.AppWindow.Size.Height);
                }

                additionalHeight = WND_FRAME_TOP_NORMAL; // Required for window drag region
                isMaximized = false; // Required for NCHITTEST

                if (wasMaximized != isMaximized)
                {
                    if (IsLeftMouseButtonDown()) MoveCursorUp();
                    SwitchState(ButtonsState.None);
                }
            }

            // Local method to handle unknown presenter states
            void HandleUnknownState()
            {
                if (MemorizeWindowPosition) SetValue($"{WindowTag}Maximized", true);

                additionalHeight = 0; // Required for window drag region
                isMaximized = false; // Required for NCHITTEST
            }
        }

        public async void UpdateWindowProperties()
        {
            try
            {
                // Update window capabilities
                CanMaximize = !isMaximized && IsMaximizable;
                CanMove = !isMaximized;
                CanSize = CurrentWindow is not null && !isMaximized && CurrentWindow.IsResizable;
                CanRestore = isMaximized && IsMaximizable;

                if (MinimizeButton is not null && MaximizeRestoreButton is not null && Application.Current is not null && CloseButton is not null)
                {
                    if (CurrentWindow is not null)
                    {
                        // Maximize
                        CurrentWindow.IsMaximizable = IsMaximizable;
                        MaximizeRestoreButton.IsEnabled = IsMaximizable;

                        // Minimize
                        CurrentWindow.IsMinimizable = IsMinimizable;
                        MinimizeButton.IsEnabled = IsMinimizable;

                        // Close
                        CloseButton.IsEnabled = IsClosable;
                    }

                    CheckMaximization();

                    // Update button visibility and styles
                    SetButtonVisibility(
                        // Check if the buttons are both disabled
                        !IsMinimizable && !IsMaximizable ?

                        // If yes, hide them
                        Visibility.Collapsed :

                        // If not, keep them open
                        Visibility.Visible,

                        // Check if the buttons are both disabled
                        !IsMinimizable && !IsMaximizable ?

                        // If yes, change the style of the close button
                        "CloseSingular" :

                        // If not, restore the original close button style
                        "Close");
                }

                // Repeat
                await Task.Delay(50);
                UpdateWindowBrushes();
                UpdateWindowProperties();
            }
            catch
            {

            }

            // Local method to update button visibility and style
            void SetButtonVisibility(Visibility visibility, string closeStyleKey)
            {
                MinimizeButton.Visibility = MaximizeRestoreButton.Visibility = visibility;
                CloseButton.Style = Application.Current.Resources[closeStyleKey] as Style;
            }
        }

        private async void UpdateWindowSizeAndPosition()
        {
            // Exit if window position memory is disabled
            if (!MemorizeWindowPosition) return;

            // Prevent unnecessary size checks
            allowSizeCheck = false;

            // Check if the window position is saved
            if (GetValue<object>($"{WindowTag}PositionX") is not null)
            {
                // Move and resize the window based on saved values
                MoveAndResize();

                // If the window was maximized, restore and maximize it again
                if (GetValue<object>($"{WindowTag}Maximized") is bool and true)
                {
                    // Allow some time for the move/resize to take effect
                    await Task.Delay(10);

                    // Maximize the window
                    CurrentWindow?.Maximize();
                    isMaximized = true;
                }
            }

            // Ensure window is maximized if the value is set to true
            if (GetValue<object>($"{WindowTag}Maximized") is bool maximized && maximized)
            {
                CurrentWindow?.Maximize();
                isMaximized = true;
            }

            // Allow size checks to resume
            allowSizeCheck = true;

            // Small delay before switching state
            await Task.Delay(200);

            // Reset button states
            SwitchState(ButtonsState.None);

            // Local method for applying dimensions
            void MoveAndResize()
            {
                CurrentWindow?.MoveAndResize(
                    GetValue<double>($"{WindowTag}PositionX") / Display.Scale(CurrentWindow),
                    GetValue<double>($"{WindowTag}PositionY") / Display.Scale(CurrentWindow),
                    GetValue<double>($"{WindowTag}Width") / Display.Scale(CurrentWindow),
                    GetValue<double>($"{WindowTag}Height") / Display.Scale(CurrentWindow));
            }
        }

        public void UpdateWindowBrushes()
        {
            // If the window has been closed stop checking
            if (closed) return;

            // Determine the appropriate foreground brush
            SolidColorBrush? focusedForeground = Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
            SolidColorBrush? unfocusedForeground = Application.Current.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;

            // Update based on accent title bar settings
            if (IsAccentColorEnabledForTitleBars() && IsAccentTitleBarEnabled)
            {
                // Accent enabled
                if (AccentStrip is not null) UpdateAccentVisibility(isWindowFocused);

                CurrentForeground = isWindowFocused ?
                    // If the window is focused, make the buttons white
                    new SolidColorBrush(Colors.White) :

                    // If the window is not focused, sync buttons with theme
                    unfocusedForeground;

                Application.Current.Resources["CaptionForegroundInteract"] =
                    isWindowFocused ?

                    // If the window is focused, make the buttons white
                    Colors.White :

                    // If the window is not focused, sync buttons with theme
                    focusedForeground?.Color;
            }
            else
            {
                // Accent disabled
                if (AccentStrip is not null) UpdateAccentVisibility(false);

                CurrentForeground = isWindowFocused ?
                    // If the window is focused, make the buttons a solid color (theme synced)
                    focusedForeground :

                    // If the window is not focused, sync buttons with theme
                    unfocusedForeground;

                Application.Current.Resources["CaptionForegroundInteract"] =
                    // Doesn't require special handling
                    focusedForeground?.Color;
            }

            // Local method to toggle AccentStrip visibility
            void UpdateAccentVisibility(bool isVisible) => AccentStrip.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public static class WindowExtensions
    {
        // Window messages
        private const int WM_SYSCOMMAND = 0x0112; // System command

        // System commands
        private const int SC_MOVE = 0xF010;       // Move window
        private const int SC_SIZE = 0xF000;       // Resize window

        public static IntPtr GetHwnd(this WindowEx windowEx)
        {
            // Get the native window handle (HWND)
            return WinRT.Interop.WindowNative.GetWindowHandle(windowEx);
        }

        public static void InvokeResize(this WindowEx windowEx)
        {
            PostMessage(windowEx.GetHwnd(), WM_SYSCOMMAND, (IntPtr)SC_SIZE, IntPtr.Zero);
        }

        public static void InvokeMove(this WindowEx windowEx)
        {
            PostMessage(windowEx.GetHwnd(), WM_SYSCOMMAND, (IntPtr)SC_MOVE, IntPtr.Zero);
        }

        // Native methods

        // Importing PostMessage from user32.dll to send a message to the specified window
        [DllImport(Libraries.User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}