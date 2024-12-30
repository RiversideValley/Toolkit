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
        protected Button CloseButton { get; private set; }
        protected ToggleButton MaximizeRestoreButton { get; private set; }
        protected Button MinimizeButton { get; private set; }
        protected TextBlock TitleTextBlock { get; private set; }
        protected ImageIcon TitleBarIcon { get; private set; }
        protected WindowEx CurrentWindow { get; private set; }
        protected Application CurrentApp { get; private set; }
        protected Border AccentStrip { get; private set; }
        protected MenuFlyout CustomRightClickFlyout { get; private set; }

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
                        GetValue<double>($"{WindowTag}PositionX") / Display.Scale(CurrentWindow),
                        GetValue<double>($"{WindowTag}PositionY") / Display.Scale(CurrentWindow),
                        GetValue<double>($"{WindowTag}Width") / Display.Scale(CurrentWindow),
                        GetValue<double>($"{WindowTag}Height") / Display.Scale(CurrentWindow));

                    await Task.Delay(10);

                    CurrentWindow.Maximize();
                    isMaximized = true;
                }
                else
                {
                    CurrentWindow.MoveAndResize(
                        GetValue<double>($"{WindowTag}PositionX") / Display.Scale(CurrentWindow),
                        GetValue<double>($"{WindowTag}PositionY") / Display.Scale(CurrentWindow),
                        GetValue<double>($"{WindowTag}Width") / Display.Scale(CurrentWindow),
                        GetValue<double>($"{WindowTag}Height") / Display.Scale(CurrentWindow));
                }
            }
            if (GetValue<object>($"{WindowTag}Maximized") != null)
            {
                if (GetValue<bool>($"{WindowTag}Maximized") == true)
                {
                    CurrentWindow.Maximize();
                    isMaximized = true;
                }
                else
                {

                }
            }

            allowSizeCheck = true;

            await Task.Delay(200);

            SwitchState(ButtonsState.None);
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
            args.Handled = !IsClosable;
            closed = IsClosable;
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
            (GetTemplateChild("MaximizeContextMenuItem") as MenuFlyoutItem).Click += MaximizeContextMenu_Click;
            (GetTemplateChild("SizeContextMenuItem") as MenuFlyoutItem).Click += SizeContextMenu_Click;
            (GetTemplateChild("MoveContextMenuItem") as MenuFlyoutItem).Click += MoveContextMenu_Click;
            (GetTemplateChild("MinimizeContextMenuItem") as MenuFlyoutItem).Click += MinimizeContextMenu_Click;
            (GetTemplateChild("CloseContextMenuItem") as MenuFlyoutItem).Click += CloseContextMenu_Click;
            (GetTemplateChild("RestoreContextMenuItem") as MenuFlyoutItem).Click += RestoreContextMenu_Click;
        }

        public const string AccentRegistryKeyPath = @"Software\Microsoft\Windows\DWM";
        public const string AccentRegistryValueName = "ColorPrevalence";

        public async void CheckWindow()
        {
            try
            {
                CanMaximize = !isMaximized && IsMaximizable;
                CanMove = !isMaximized;
                CanSize = !isMaximized && CurrentWindow.IsResizable;
                CanRestore = isMaximized && IsMaximizable;
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
    }
}