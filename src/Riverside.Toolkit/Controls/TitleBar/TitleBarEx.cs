using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Threading.Tasks;
using WinUIEx;
using WinUIEx.Messaging;
using static Riverside.Toolkit.Helpers.NativeHelper;
using Microsoft.Windows.Storage;

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx : Control
    {
        protected Button CloseButton { get; set; }
        protected ToggleButton MaximizeRestoreButton { get; set; }
        protected Button MinimizeButton { get; set; }
        protected TextBlock TitleTextBlock { get; set; }
        protected ImageIcon TitleBarIcon { get; set; }
        protected WindowEx CurrentWindow { get; set; }
        protected Application CurrentApp { get; set; }
        protected Border AccentStrip { get; set; }
        protected MenuFlyout CustomRightClickFlyout { get; set; }
        private WindowMessageMonitor messageMonitor;
        private bool isWindowFocused = false;
        private bool isMaximized = false;
        private int buttonDownHeight = 0;
        private double additionalHeight = 0;
        protected SelectedCaptionButton currentCaption = SelectedCaptionButton.None;
        private bool closed = false;
        private bool allowSizeCheck = false;

        private static T GetValue<T>(string key)
        {
            try
            {
                var userSettings = ApplicationData.GetDefault();
                return (T)userSettings.LocalSettings.Values[key];
            }
            catch
            {
                return default;
            }
        }

        private static void SetValue<T>(string key, T newValue)
        {
            try
            {
                var userSettings = ApplicationData.GetDefault();
                userSettings.LocalSettings.Values[key] = newValue;
            }
            catch
            {
                return;
            }
        }

        private async void CheckWindowProperties()
        {
            if (!MemorizeWindowPosition) return;

            allowSizeCheck = false;

            if (GetValue<object>($"{WindowTag}PositionX") != null)
            {
                if (GetValue<object>($"{WindowTag}Maximized") is bool and true)
                {
                    CurrentWindow.MoveAndResize(
                        GetValue<double>($"{WindowTag}PositionX"),
                        GetValue<double>($"{WindowTag}PositionY"),
                        GetValue<double>($"{WindowTag}Width"),
                        GetValue<double>($"{WindowTag}Height"));

                    await Task.Delay(10);

                    CurrentWindow.Maximize();
                }
                else
                {
                    CurrentWindow.MoveAndResize(
                        GetValue<double>($"{WindowTag}PositionX"),
                        GetValue<double>($"{WindowTag}PositionY"),
                        GetValue<double>($"{WindowTag}Width"),
                        GetValue<double>($"{WindowTag}Height"));
                }
            }
            if (GetValue<object>($"{WindowTag}Maximized") != null)
            {
                if (GetValue<bool>($"{WindowTag}Maximized") == true)
                {
                    CurrentWindow.Maximize();
                }
                else
                {

                }
            }

            allowSizeCheck = true;

            CheckMaximization();
        }

        public TitleBarEx()
        {
            DefaultStyleKey = typeof(TitleBarEx);
        }

        public void InitializeForWindow(WindowEx windowEx, Application app)
        {
            CurrentWindow = windowEx;
            CurrentApp = app;

            CurrentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            CurrentWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;

            CurrentWindow.Content.PointerMoved += Content_PointerMoved;
            CurrentWindow.Content.PointerReleased += Content_PointerReleased;
            CurrentWindow.Content.PointerExited += Content_PointerExited;
            CurrentWindow.Content.PointerEntered += Content_PointerEntered;
            PointerExited += TitleBarEx_PointerExited;

            CurrentWindow.WindowStateChanged += CurrentWindow_WindowStateChanged;

            CurrentWindow.Closed += CurrentWindow_Closed;

            CheckWindowProperties();
            CheckWindow();
            Rehook();
            LoadBounds();
        }

        private void TitleBarEx_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // No buttons are selected
            SwitchState(ButtonsState.None);
        }

        private void Content_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // No buttons are selected
            SwitchState(ButtonsState.None);
        }

        private void CurrentWindow_Closed(object sender, WindowEventArgs args)
        {
            closed = true;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            CloseButton = GetTemplateChild("CloseButton") as Button;
            MaximizeRestoreButton = GetTemplateChild("MaximizeButton") as ToggleButton;
            MinimizeButton = GetTemplateChild("MinimizeButton") as Button;
            TitleTextBlock = GetTemplateChild("TitleTextBlock") as TextBlock;
            TitleBarIcon = GetTemplateChild("TitleBarIcon") as ImageIcon;
            AccentStrip = GetTemplateChild("AccentStrip") as Border;
            CustomRightClickFlyout = GetTemplateChild("CustomRightClickFlyout") as MenuFlyout;
        }

        public const string AccentRegistryKeyPath = @"Software\Microsoft\Windows\DWM";
        public const string AccentRegistryValueName = "ColorPrevalence";

        public async void CheckWindow()
        {
            try
            {
                if (MinimizeButton is not null && MaximizeRestoreButton is not null && CurrentApp is not null && CloseButton is not null)
                {
                    if (CurrentWindow is not null)
                    {
                        CurrentWindow.IsMaximizable = IsMaximizable;
                        MaximizeRestoreButton.IsEnabled = IsMaximizable;
                        CurrentWindow.IsMinimizable = IsMinimizable;
                        MinimizeButton.IsEnabled = IsMinimizable;

                        CloseButton.IsEnabled = IsClosable;

                        CheckMaximization();
                    }
                    if (!IsMinimizable && !IsMaximizable)
                    {
                        MinimizeButton.Visibility = MaximizeRestoreButton.Visibility = Visibility.Collapsed;
                        CloseButton.Style = CurrentApp.Resources["CloseSingular"] as Style;
                    }
                    else
                    {
                        MinimizeButton.Visibility = MaximizeRestoreButton.Visibility = Visibility.Visible;
                        CloseButton.Style = CurrentApp.Resources["Close"] as Style;
                    }
                }
                await Task.Delay(50);
                CheckFocus();
                CheckWindow();
            }
            catch
            {

            }
        }

        private void CurrentWindow_WindowStateChanged(object sender, WindowState e) => CheckMaximization();

        private void Content_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            // No buttons are selected
            SwitchState(ButtonsState.None);
            if (!IsLeftMouseButtonDown())
            {
                currentCaption = SelectedCaptionButton.None;
            }
        }

        private void Content_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // No buttons are selected
            SwitchState(ButtonsState.None);
            if (!IsLeftMouseButtonDown())
            {
                currentCaption = SelectedCaptionButton.None;
            }
        }

        private void Content_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // No buttons are selected
            SwitchState(ButtonsState.None);
            try
            {
                if (CurrentWindow != null)
                {
                    if (CurrentWindow.Content != null)
                    {
                        if (!IsLeftMouseButtonDown())
                        {
                            currentCaption = SelectedCaptionButton.None;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private async void Rehook()
        {
            if (closed) return;

            messageMonitor ??= new WindowMessageMonitor(CurrentWindow);
            messageMonitor.WindowMessageReceived -= WndProc;
            messageMonitor.WindowMessageReceived += WndProc;

            await Task.Delay(750);

            Rehook();
        }

        public void SetWindowIcon(string path)
        {
            try
            {
                if (TitleBarIcon is not null) TitleBarIcon.Source = new BitmapImage(new Uri($"{AppContext.BaseDirectory}\\{path}", UriKind.RelativeOrAbsolute));
            }
            catch
            {

            }
            CurrentWindow.SetIcon($"{AppContext.BaseDirectory}\\{path}");
        }

        public WindowEx GetWindow() => CurrentWindow;

        public static bool IsAccentColorEnabledForTitleBars()
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(AccentRegistryKeyPath);
                if (key != null)
                {
                    var value = key.GetValue(AccentRegistryValueName);
                    if (value is int intValue)
                    {
                        // ColorPrevalence value of 1 means the accent color is used for title bars and window borders.
                        return intValue == 1;
                    }
                }
            }
            catch
            {

            }

            return false; // Default to false if any issues occur
        }

        public void CheckFocus()
        {
            if (TitleTextBlock is null) return;
            if (IsAccentColorEnabledForTitleBars() && IsAccentTitleBarEnabled)
            {
                if (AccentStrip is not null) AccentStrip.Visibility = isWindowFocused ? Visibility.Visible : Visibility.Collapsed;
                CurrentForeground = isWindowFocused ? new SolidColorBrush(Colors.White) : CurrentApp.Resources["AccentTextFillColorDisabledBrush"] as SolidColorBrush;
                CurrentApp.Resources["CaptionForegroundInteract"] = isWindowFocused ? Colors.White : ((SolidColorBrush)CurrentApp.Resources["TextFillColorPrimaryBrush"]).Color;
            }
            else
            {
                if (AccentStrip is not null) AccentStrip.Visibility = Visibility.Collapsed;
                CurrentForeground = isWindowFocused ? CurrentApp.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush : CurrentApp.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;
                CurrentApp.Resources["CaptionForegroundInteract"] = ((SolidColorBrush)CurrentApp.Resources["TextFillColorPrimaryBrush"]).Color;
            }
        }

        private static bool IsLeftMouseButtonDown()
        {
            // The high-order bit indicates if the key is down
            return (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }

        private async void CheckMaximization()
        {
            if (closed || !allowSizeCheck) return;

            var _isMaximized = isMaximized;
            if (CurrentWindow.Presenter is OverlappedPresenter presenter)
            {
                switch (presenter.State)
                {
                    case OverlappedPresenterState.Maximized:
                        {
                            if (MemorizeWindowPosition) SetValue($"{WindowTag}Maximized", true);

                            await Task.Delay(10);

                            // Required for window drag region
                            additionalHeight = WND_FRAME_TOP_MAXIMIZED;

                            // Required for NCHITTEST
                            isMaximized = true;

                            if (!_isMaximized) SwitchState(ButtonsState.None);

                            break;
                        }
                    case OverlappedPresenterState.Restored:
                        {
                            if (MemorizeWindowPosition)
                            {
                                SetValue($"{WindowTag}Maximized", false);
                                SetValue<double>($"{WindowTag}PositionX", CurrentWindow.AppWindow.Position.X);
                                SetValue<double>($"{WindowTag}PositionY", CurrentWindow.AppWindow.Position.Y);
                                SetValue<double>($"{WindowTag}Width", CurrentWindow.AppWindow.Size.Width);
                                SetValue<double>($"{WindowTag}Height", CurrentWindow.AppWindow.Size.Height);
                            }

                            await Task.Delay(10);

                            // Required for window drag region
                            additionalHeight = WND_FRAME_TOP_NORMAL;

                            // Required for NCHITTEST
                            isMaximized = false;

                            if (_isMaximized) SwitchState(ButtonsState.None);

                            break;
                        }
                }
            }
            else
            {
                if (MemorizeWindowPosition) SetValue($"{WindowTag}Maximized", true);

                await Task.Delay(10);

                // Required for window drag region
                additionalHeight = 0;

                // Required for NCHITTEST
                isMaximized = false;

                return;
            }
        }

        protected void SwitchState(ButtonsState buttonsState)
        {
            CheckMaximization();

            if (CloseButton is null || MaximizeRestoreButton is null || MinimizeButton is null) return;

            string minimizeState = string.Empty;
            string maximizeState = string.Empty;
            string closeState = string.Empty;

            switch (buttonsState)
            {
                case ButtonsState.None:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "Normal" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "Normal" : "Disabled";

                        break;
                    }
                case ButtonsState.MinimizePointerOver:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "PointerOver" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "Normal" : "Disabled";

                        break;
                    }
                case ButtonsState.MinimizePressed:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "Pressed" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "Normal" : "Disabled";

                        break;
                    }
                case ButtonsState.MaximizePointerOver:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "Normal" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "Normal" : "Disabled";
                        break;
                    }
                case ButtonsState.MaximizePressed:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "Normal" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "Normal" : "Disabled";
                        break;
                    }
                case ButtonsState.ClosePointerOver:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "Normal" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "PointerOver" : "Disabled";
                        break;
                    }
                case ButtonsState.ClosePressed:
                    {
                        // Minimize
                        minimizeState = IsMinimizable ? "Normal" : "Disabled";

                        // Maximize
                        CheckMaximizeNormalStates();

                        // Close
                        closeState = IsClosable ? "Pressed" : "Disabled";
                        break;
                    }
            }

            if (UseWinUIEverywhere)
            {
                switch (buttonsState)
                {
                    case ButtonsState.MinimizePointerOver:
                        {
                            (ToolTipService.GetToolTip(MinimizeButton) as ToolTip).IsOpen = true;
                            (ToolTipService.GetToolTip(CloseButton) as ToolTip).IsOpen = false;
                            break;
                        }
                    case ButtonsState.ClosePointerOver:
                        {
                            (ToolTipService.GetToolTip(MinimizeButton) as ToolTip).IsOpen = false;
                            (ToolTipService.GetToolTip(CloseButton) as ToolTip).IsOpen = true;
                            break;
                        }
                    default:
                        {
                            (ToolTipService.GetToolTip(MinimizeButton) as ToolTip).IsOpen = false;
                            (ToolTipService.GetToolTip(CloseButton) as ToolTip).IsOpen = false;
                            break;
                        }
                }
            }

            _ = VisualStateManager.GoToState(MinimizeButton, minimizeState, true);
            _ = VisualStateManager.GoToState(MaximizeRestoreButton, maximizeState, true);
            _ = VisualStateManager.GoToState(CloseButton, closeState, true);

            void CheckMaximizeNormalStates()
            {
                switch (IsMaximizable)
                {
                    // Can maximize
                    case true:
                        {
                            maximizeState = isMaximized ? "Checked" : "Normal";
                            if (buttonsState == ButtonsState.MaximizePointerOver)
                            {
                                maximizeState = isMaximized ? "CheckedPointerOver" : "PointerOver";
                                break;
                            }
                            else if (buttonsState == ButtonsState.MaximizePressed)
                            {
                                maximizeState = isMaximized ? "CheckedPressed" : "Pressed";
                                break;
                            }
                            break;
                        }

                    // Can't maximize
                    case false:
                        {
                            maximizeState = isMaximized ? "CheckedDisabled" : "Disabled";
                            break;
                        }
                }
            }
        }

        private async void LoadBounds()
        {
            // Make sure the loop doesn't trigger too often
            await Task.Delay(100);

            try
            {
                // If the window has been closed break the loop
                if (closed) return;

                // Check if every condition is met
                if (CurrentWindow.AppWindow is not null && IsAutoDragRegionEnabled)
                {
                    // Width (Scaled window width)
                    var width = (int)(CurrentWindow.Bounds.Width * Display.Scale(CurrentWindow));

                    // Height (Scaled control actual height)
                    var height = (int)((ActualHeight + buttonDownHeight) * Display.Scale(CurrentWindow));

                    CurrentWindow.AppWindow.TitleBar.SetDragRectangles([new(0, 0, width, height)]);
                }
                else
                {

                }

                LoadBounds();
            }
            catch
            {
                return;
            }
        }
    }
}