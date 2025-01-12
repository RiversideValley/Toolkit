#if WinUI
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Riverside.ComponentModel;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Graphics;
using WinUIEx;
using static Riverside.Toolkit.Helpers.NativeHelper;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar;

public partial class TitleBarEx
{
    private void InvokeChecks()
    {
        UpdateWindowProperties();
        CheckMaximization();
        LoadDragRegion();
    }

    private void LoadDragRegion()
    {
        try
        {
            // If the window has been closed, break the loop
            if (closed) return;

            // Check if every condition is met
            if (this.CurrentWindow?.AppWindow is not null && this.IsAutoDragRegionEnabled)
            {
                // Width (Scaled window width)
                int width = (int)(this.CurrentWindow.Bounds.Width * Display.Scale(this.CurrentWindow));

                // Height (Scaled control actual height)
                int height = (int)((this.ActualHeight + buttonDownHeight) * Display.Scale(this.CurrentWindow));

                // Set the drag region for the window's title bar
                this.CurrentWindow.AppWindow.TitleBar.SetDragRectangles([new RectInt32(0, 0, width, height)]);
            }
        }
        catch
        {
            return;
        }
    }

    public void SetWindowIcon(string path)
    {
        try
        {
            // Attempt to set the title bar icon
            if (this.TitleBarIcon is not null)
            {
                var uri = new Uri($"ms-appx:///{path}", UriKind.RelativeOrAbsolute);
                this.TitleBarIcon.Source = new BitmapImage(uri);
            }
        }
        catch
        {

        }

        try
        {
            // Set the window icon
            string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, path);
            this.CurrentWindow?.SetIcon(iconPath);
        }
        catch
        {

        }
    }

    private bool wasMaximized = false;

    private void CheckMaximization()
    {
        if (closed || !allowSizeCheck) return;

        if (this.CurrentWindow?.Presenter is OverlappedPresenter presenter)
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
            if (this.MemorizeWindowPosition) SetValue($"{this.WindowTag}Maximized", true);

            additionalHeight = WND_FRAME_TOP_MAXIMIZED; // Required for window drag region
            isMaximized = true; // Required for NCHITTEST

            if (wasMaximized != isMaximized) SwitchState(ButtonsState.None);
        }

        // Local method to handle the restored state
        void HandleRestoredState()
        {
            if (this.MemorizeWindowPosition)
            {
                SetValue($"{this.WindowTag}Maximized", false);
                SetValue<double>($"{this.WindowTag}PositionX", this.CurrentWindow.AppWindow.Position.X);
                SetValue<double>($"{this.WindowTag}PositionY", this.CurrentWindow.AppWindow.Position.Y);
                SetValue<double>($"{this.WindowTag}Width", this.CurrentWindow.AppWindow.Size.Width);
                SetValue<double>($"{this.WindowTag}Height", this.CurrentWindow.AppWindow.Size.Height);
            }

            additionalHeight = WND_FRAME_TOP_NORMAL; // Required for window drag region
            isMaximized = false; // Required for NCHITTEST

            if (wasMaximized != isMaximized)
            {
                SwitchState(ButtonsState.None);
            }
        }

        // Local method to handle unknown presenter states
        void HandleUnknownState()
        {
            if (this.MemorizeWindowPosition) SetValue($"{this.WindowTag}Maximized", true);

            additionalHeight = 0; // Required for window drag region
            isMaximized = false; // Required for NCHITTEST
        }
    }

    public void UpdateWindowProperties()
    {
        try
        {
            // Update window capabilities
            this.CanMaximize = !isMaximized && this.IsMaximizable;
            this.CanMove = !isMaximized;
            this.CanSize = this.CurrentWindow is not null && !isMaximized && this.CurrentWindow.IsResizable;
            this.CanRestore = isMaximized && this.IsMaximizable;

            if (this.MinimizeButton is not null && this.MaximizeRestoreButton is not null && Application.Current is not null && this.CloseButton is not null)
            {
                if (this.CurrentWindow is not null)
                {
                    // Maximize
                    this.CurrentWindow.IsMaximizable = this.IsMaximizable;
                    this.MaximizeRestoreButton.IsEnabled = this.IsMaximizable;

                    // Minimize
                    this.CurrentWindow.IsMinimizable = this.IsMinimizable;
                    this.MinimizeButton.IsEnabled = this.IsMinimizable;

                    // Close
                    this.CloseButton.IsEnabled = this.IsClosable;
                }

                CheckMaximization();

                // Update button visibility and styles
                SetButtonVisibility(
                    // Check if the buttons are both disabled
                    !this.IsMinimizable && !this.IsMaximizable ?

                    // If yes, hide them
                    Visibility.Collapsed :

                    // If not, keep them open
                    Visibility.Visible,

                    // Check if the buttons are both disabled
                    !this.IsMinimizable && !this.IsMaximizable ?

                    // If yes, change the style of the close button
                    this.CloseButtonSingularStyleKey :

                    // If not, restore the original close button style
                    this.CloseButtonRegularStyleKey);
            }
        }
        catch
        {

        }

        // Local method to update button visibility and style
        void SetButtonVisibility(Visibility visibility, string? closeStyleKey)
        {
            this.MinimizeButton.Visibility = this.MaximizeRestoreButton.Visibility = visibility;
            this.CloseButton.Style = Application.Current.Resources[closeStyleKey] as Style;
        }
    }

    private async void UpdateWindowSizeAndPosition()
    {
        // Exit if window position memory is disabled
        if (!this.MemorizeWindowPosition) return;

        // Prevent unnecessary size checks
        allowSizeCheck = false;

        // Check if the window position is saved
        if (GetValue<object>($"{this.WindowTag}PositionX") is not null)
        {
            // Move and resize the window based on saved values
            MoveAndResize();

            // If the window was maximized, restore and maximize it again
            if (GetValue<object>($"{this.WindowTag}Maximized") is bool and true)
            {
                // Allow some time for the move/resize to take effect
                await Task.Delay(10);

                // Maximize the window
                this.CurrentWindow?.Maximize();
                isMaximized = true;
            }
        }

        // Ensure window is maximized if the value is set to true
        if (GetValue<object>($"{this.WindowTag}Maximized") is bool maximized && maximized)
        {
            this.CurrentWindow?.Maximize();
            isMaximized = true;
        }

        // Allow size checks to resume
        allowSizeCheck = true;

        // Small delay before switching state
        await Task.Delay(200);

        // Reset button states
        SwitchState(ButtonsState.None);

        // Local method for applying dimensions
        void MoveAndResize() => this.CurrentWindow?.MoveAndResize(
                GetValue<double>($"{this.WindowTag}PositionX") / Display.Scale(this.CurrentWindow),
                GetValue<double>($"{this.WindowTag}PositionY") / Display.Scale(this.CurrentWindow),
                GetValue<double>($"{this.WindowTag}Width") / Display.Scale(this.CurrentWindow),
                GetValue<double>($"{this.WindowTag}Height") / Display.Scale(this.CurrentWindow));
    }

    public void UpdateWindowBrushes()
    {
        // If the window has been closed stop checking
        if (closed) return;

        // Determine the appropriate foreground brush
        var focusedForeground = Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;
        var unfocusedForeground = Application.Current.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;

        // Update based on accent title bar settings
        if (IsAccentColorEnabledForTitleBars() && this.IsAccentTitleBarEnabled)
        {
            // Accent enabled
            if (this.AccentStrip is not null) UpdateAccentVisibility(isWindowFocused);

            this.CurrentForeground = isWindowFocused ?
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
            if (this.AccentStrip is not null) UpdateAccentVisibility(false);

            this.CurrentForeground = isWindowFocused ?
                // If the window is focused, make the buttons a solid color (theme synced)
                focusedForeground :

                // If the window is not focused, sync buttons with theme
                unfocusedForeground;

            Application.Current.Resources["CaptionForegroundInteract"] =
                // Doesn't require special handling
                focusedForeground?.Color;
        }

        // Local method to toggle AccentStrip visibility
        void UpdateAccentVisibility(bool isVisible) => this.AccentStrip.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
    }
}

public static class WindowExtensions
{
    // Window messages
    private const int WM_SYSCOMMAND = 0x0112; // System command

    // System commands
    private const int SC_MOVE = 0xF010;       // Move window
    private const int SC_SIZE = 0xF000;       // Resize window

    public static IntPtr GetHwnd(this WindowEx windowEx) =>
        // Get the native window handle (HWND)
        WinRT.Interop.WindowNative.GetWindowHandle(windowEx);

    public static void InvokeResize(this WindowEx windowEx) => PostMessage(windowEx.GetHwnd(), WM_SYSCOMMAND, SC_SIZE, IntPtr.Zero);

    public static void InvokeMove(this WindowEx windowEx) => PostMessage(windowEx.GetHwnd(), WM_SYSCOMMAND, SC_MOVE, IntPtr.Zero);

    // Native methods

    // Importing PostMessage from user32.dll to send a message to the specified window
    [DllImport(Libraries.User32, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}
#endif