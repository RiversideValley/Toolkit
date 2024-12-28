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
using Windows.Graphics;
using WinUIEx;
using WinUIEx.Messaging;
using static Riverside.Toolkit.Helpers.NativeHelper;
using DependencyPropertyGenerator;
using System.Diagnostics;
using CommunityToolkit.WinUI.UI.Controls;
using System.Linq;
using System.Diagnostics.Metrics;
using Windows.UI;

namespace Riverside.Toolkit.Controls
{
    [DependencyProperty<bool>("IsAutoDragRegionEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsAccentTitleBarEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsMinimizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsMaximizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsClosable", DefaultValue = true)]
    [DependencyProperty<bool>("UseWinUIEverywhere", DefaultValue = false)]
    [DependencyProperty<Color>("CaptionForegroundInteract")]
    [DependencyProperty<SolidColorBrush>("CurrentForeground")]
    [DependencyProperty<string>("Title")]
    [DependencyProperty<string>("Subtitle")]
    public partial class TitleBarEx : Control
    {
        private Button CloseButton { get; set; }
        private ToggleButton MaximizeRestoreButton { get; set; }
        private Button MinimizeButton { get; set; }
        private Button HelpButton { get; set; }
        private TextBlock TitleTextBlock { get; set; }
        private TextBlock SubtitleBlock { get; set; }
        private ImageIcon TitleBarIcon { get; set; }
        private WindowEx CurrentWindow { get; set; }
        private Application CurrentApp { get; set; }
        private Border AccentStrip { get; set; }
        private WindowMessageMonitor MessageMonitor { get; set; }
        private bool isWindowFocused { get; set; } = false;
        private bool isMaximized { get; set; } = false;
        private int buttonDownHeight { get; set; } = 0;
        private IntPtr lastPos { get; set; }
        private double additionalHeight { get; set; } = 0;
        private SelectedCaptionButton currentCaption { get; set; } = SelectedCaptionButton.None;
        private bool closed { get; set; }

        public enum SelectedCaptionButton
        {
            None = 0,
            Minimize = 1,
            Maximize = 2,
            Close = 3
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
            //HelpButton = GetTemplateChild("HelpButton") as Button;
            TitleTextBlock = GetTemplateChild("TitleTextBlock") as TextBlock;
            SubtitleBlock = GetTemplateChild("SubtitleTextBlock") as TextBlock;
            TitleBarIcon = GetTemplateChild("TitleBarIcon") as ImageIcon;
            AccentStrip = GetTemplateChild("AccentStrip") as Border;
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

        public static bool IsInRect(double x, double xMin, double xMax, double y, double yMin, double yMax) => xMin <= x && x <= xMax && yMin <= y && y <= yMax;

        private async void Rehook()
        {
            MessageMonitor ??= new WindowMessageMonitor(CurrentWindow);
            MessageMonitor.WindowMessageReceived -= Event;
            MessageMonitor.WindowMessageReceived += Event;

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
                if (AccentStrip is not null) AccentStrip.Visibility = !isWindowFocused ? Visibility.Visible : Visibility.Collapsed;
                CurrentForeground = !isWindowFocused ? new SolidColorBrush(Colors.White) : CurrentApp.Resources["AccentTextFillColorDisabledBrush"] as SolidColorBrush;
                CurrentApp.Resources["CaptionForegroundInteract"] = !isWindowFocused ? Colors.White : ((SolidColorBrush)CurrentApp.Resources["TextFillColorPrimaryBrush"]).Color;
            }
            else
            {
                if (AccentStrip is not null) AccentStrip.Visibility = Visibility.Collapsed;
                CurrentForeground = !isWindowFocused ? CurrentApp.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush : CurrentApp.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;
                CurrentApp.Resources["CaptionForegroundInteract"] = ((SolidColorBrush)CurrentApp.Resources["TextFillColorPrimaryBrush"]).Color;
            }
        }

        private async void Event(object sender, WindowMessageEventArgs e)
        {
            if (closed) return;

            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int WM_NCHITTEST = 0x0084;
            const int WM_NCLBUTTONUP = 0x00A2;
            const int WM_ACTIVATE = 0x0006;
            const int WA_INACTIVE = 0;

            // Gets the pointer's position relative to the screen's edge with DPI scaling applied
            var x = GET_X_LPARAM(e.Message.LParam);
            var y = GET_Y_LPARAM(e.Message.LParam);

            lastPos = e.Message.LParam;

            double xMinimizeMin = 0;
            double xMinimizeMax = 0;

            double xMaximizeMin = 0;
            double xMaximizeMax = 0;

            double xCloseMin = 0;
            double xCloseMax = 0;

            double yMinimizeMin = 0;
            double yMinimizeMax = 0;
            double yMaximizeMin = 0;
            double yMaximizeMax = 0;
            double yCloseMin = 0;
            double yCloseMax = 0;

            try
            {
                if (MinimizeButton != null && MaximizeRestoreButton != null && CloseButton != null)
                {
                    var closeVisual = CloseButton.TransformToVisual(CurrentWindow.Content);
                    var maximizeRestoreVisual = MaximizeRestoreButton.TransformToVisual(CurrentWindow.Content);
                    var minimizeVisual = MinimizeButton.TransformToVisual(CurrentWindow.Content);

                    var minimizeVisualPoint = minimizeVisual.TransformPoint(new Windows.Foundation.Point(0, 0));
                    var maximizeRestoreVisualPoint = maximizeRestoreVisual.TransformPoint(new Windows.Foundation.Point(0, 0));
                    var closeVisualPoint = closeVisual.TransformPoint(new Windows.Foundation.Point(0, 0));

                    // Gets the X positions from: Window X + Window border + (Window size +/- button size)
                    xMinimizeMin =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        minimizeVisualPoint.X * Display.Scale(CurrentWindow);
                    xMinimizeMax =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        (minimizeVisualPoint.X + MinimizeButton.ActualWidth) * Display.Scale(CurrentWindow);
                    xMaximizeMin =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        maximizeRestoreVisualPoint.X * Display.Scale(CurrentWindow);
                    xMaximizeMax =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        (maximizeRestoreVisualPoint.X + MaximizeRestoreButton.ActualWidth) * Display.Scale(CurrentWindow);
                    xCloseMin =
                    CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.X * Display.Scale(CurrentWindow);
                    xCloseMax =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        (closeVisualPoint.X + CloseButton.ActualWidth) * Display.Scale(CurrentWindow);

                    // Gets the Y positions from: Window Y + Window border + (Window size +/- button size)
                    yMinimizeMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        minimizeVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yMinimizeMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        minimizeVisualPoint.Y + MinimizeButton.ActualHeight * Display.Scale(CurrentWindow);
                    yMaximizeMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        maximizeRestoreVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yMaximizeMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        maximizeRestoreVisualPoint.Y + MaximizeRestoreButton.ActualHeight * Display.Scale(CurrentWindow);
                    yCloseMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yCloseMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.Y + CloseButton.ActualHeight * Display.Scale(CurrentWindow);
                }
            }
            catch
            {
                return;
            }

            if (currentCaption is SelectedCaptionButton.None) buttonDownHeight = 0;

            switch (e.Message.MessageId)
            {
                case WM_ACTIVATE:
                    {
                        var wParam = e.Message.WParam.ToUInt32();
                        if (wParam == WA_INACTIVE)
                        {
                            // The window has lost focus
                            isWindowFocused = true;
                            CheckFocus();
                        }
                        else
                        {
                            // The window has gained focus
                            isWindowFocused = false;
                            CheckFocus();
                        }
                        break;
                    }
                case WM_NCHITTEST:
                    {
                        e.Handled = true;

                        if (IsLeftMouseButtonDown()) buttonDownHeight = 25;
                        else buttonDownHeight = 0;

                        // Minimize Button
                        if (IsInRect(x, xMinimizeMin, xMinimizeMax, y, yMinimizeMin, yMinimizeMax) && MinimizeButton.Visibility == Visibility.Visible)
                        {
                            if (!IsMinimizable)
                            {
                                // Cancel every other button
                                SwitchState(ButtonsState.None);

                                e.Result = new IntPtr(18);
                                await Task.Delay(1000);
                                e.Handled = false;
                            }

                            SwitchState(
                                // If the current caption is none, select it as usual
                                currentCaption == SelectedCaptionButton.None ? ButtonsState.MinimizePointerOver : // False state

                                // If the current caption is the button's type, select the button as pressed
                                currentCaption == SelectedCaptionButton.Minimize ? ButtonsState.MinimizePressed : // False state

                                // Otherwise, this is not the button the user previously selected
                                ButtonsState.None);

                            if (currentCaption is SelectedCaptionButton.None && IsMinimizable)
                            {
                                e.Result = new IntPtr(UseWinUIEverywhere ? 18 : 8);
                                await Task.Delay(800);
                                e.Handled = false;
                            }
                        }

                        // Maximize Button
                        else if (IsInRect(x, xMaximizeMin, xMaximizeMax, y, yMaximizeMin, yMaximizeMax) && MaximizeRestoreButton.Visibility == Visibility.Visible)
                        {
                            if (!IsMaximizable)
                            {
                                // Cancel every other button
                                SwitchState(ButtonsState.None);

                                e.Result = new IntPtr(18);
                                await Task.Delay(1000);
                                e.Handled = false;
                            }

                            SwitchState(
                                // If the current caption is none, select it as usual
                                currentCaption == SelectedCaptionButton.None ? ButtonsState.MaximizePointerOver : // False state

                                // If the current caption is the button's type, select the button as pressed
                                currentCaption == SelectedCaptionButton.Maximize ? ButtonsState.MaximizePressed : // False state

                                // Otherwise, this is not the button the user previously selected
                                ButtonsState.None);

                            if (currentCaption is SelectedCaptionButton.None && IsMaximizable)
                            {
                                e.Result = new IntPtr(9);
                                await Task.Delay(800);
                                e.Handled = false;
                            }
                        }

                        // Close Button
                        else if (IsInRect(x, xCloseMin, xCloseMax, y, yCloseMin, yCloseMax))
                        {
                            if (!IsClosable)
                            {
                                // Cancel every other button
                                SwitchState(ButtonsState.None);

                                e.Result = new IntPtr(18);
                                await Task.Delay(1000);
                                e.Handled = false;
                            }

                            SwitchState(
                                // If the current caption is none, select it as usual
                                currentCaption == SelectedCaptionButton.None ? ButtonsState.ClosePointerOver : // False state

                                // If the current caption is the button's type, select the button as pressed
                                currentCaption == SelectedCaptionButton.Close ? ButtonsState.ClosePressed : // False state

                                // Otherwise, this is not the button the user previously selected
                                ButtonsState.None);

                            if (currentCaption is SelectedCaptionButton.None && IsClosable)
                            {
                                e.Result = new IntPtr(UseWinUIEverywhere ? 18 : 20);
                                await Task.Delay(800);
                                e.Handled = false;
                            }
                        }

                        // Title bar drag area
                        else
                        {
                            e.Result = new IntPtr(1);

                            e.Handled = false;

                            // No buttons are selected
                            SwitchState(ButtonsState.None);

                            await Task.Delay(20);

                            break;
                        }

                        e.Handled = false;

                        break;
                    }
                case WM_NCLBUTTONDOWN:
                    {
                        e.Handled = true;

                        if (IsLeftMouseButtonDown()) buttonDownHeight = 25;
                        else buttonDownHeight = 0;

                        // Minimize Button
                        if (IsInRect(x, xMinimizeMin, xMinimizeMax, y, yMinimizeMin, yMinimizeMax) && IsLeftMouseButtonDown() && MinimizeButton.Visibility == Visibility.Visible)
                        {
                            if (!IsMinimizable)
                            {
                                // Cancel every other button
                                SwitchState(ButtonsState.None);

                                e.Handled = false;
                                return;
                            }

                            currentCaption = SelectedCaptionButton.Minimize;

                            SwitchState(
                                // If the current caption is none, select it as usual
                                currentCaption == SelectedCaptionButton.None ? ButtonsState.MinimizePointerOver : // False state

                                // If the current caption is the button's type, select the button as pressed
                                currentCaption == SelectedCaptionButton.Minimize ? ButtonsState.MinimizePressed : // False state

                                // Otherwise, this is not the button the user previously selected
                                ButtonsState.None);

                            ToolTipService.SetToolTip(MinimizeButton, null);
                        }

                        // Maximize Button
                        else if (IsInRect(x, xMaximizeMin, xMaximizeMax, y, yMaximizeMin, yMaximizeMax) && MaximizeRestoreButton.Visibility == Visibility.Visible)
                        {
                            if (!IsMaximizable)
                            {
                                // Cancel every other button
                                SwitchState(ButtonsState.None);

                                e.Handled = false;
                                return;
                            }

                            currentCaption = SelectedCaptionButton.Maximize;

                            SwitchState(
                                // If the current caption is none, select it as usual
                                currentCaption == SelectedCaptionButton.None ? ButtonsState.MaximizePointerOver : // False state

                                // If the current caption is the button's type, select the button as pressed
                                currentCaption == SelectedCaptionButton.Maximize ? ButtonsState.MaximizePressed : // False state

                                // Otherwise, this is not the button the user previously selected
                                ButtonsState.None);
                        }

                        // Close Button
                        else if (IsInRect(x, xCloseMin, xCloseMax, y, yCloseMin, yCloseMax))
                        {
                            if (!IsClosable)
                            {
                                // Cancel every other button
                                SwitchState(ButtonsState.None);

                                e.Handled = false;
                                return;
                            }

                            currentCaption = SelectedCaptionButton.Close;

                            SwitchState(
                                // If the current caption is none, select it as usual
                                currentCaption == SelectedCaptionButton.None ? ButtonsState.ClosePointerOver : // False state

                                // If the current caption is the button's type, select the button as pressed
                                currentCaption == SelectedCaptionButton.Close ? ButtonsState.ClosePressed : // False state

                                // Otherwise, this is not the button the user previously selected
                                ButtonsState.None);
                        }

                        // Title bar drag area
                        else
                        {
                            currentCaption = SelectedCaptionButton.None;

                            e.Handled = false;

                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        break;
                    }
                case WM_NCLBUTTONUP:
                    {
                        e.Handled = true;
                        e.Result = new IntPtr(1);
                        // Minimize Button
                        if (IsInRect(x, xMinimizeMin, xMinimizeMax, y, yMinimizeMin, yMinimizeMax) && currentCaption is SelectedCaptionButton.Minimize)
                        {
                            if (!IsMinimizable)
                            {
                                e.Handled = false;
                                return;
                            }
                            CurrentWindow.Minimize();

                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        // Maximize Button
                        else if (IsInRect(x, xMaximizeMin, xMaximizeMax, y, yMaximizeMin, yMaximizeMax) && currentCaption is SelectedCaptionButton.Maximize)
                        {
                            if (!IsMaximizable)
                            {
                                e.Handled = false;
                                return;
                            }
                            RunMaximization();

                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        // Close Button
                        else if (IsInRect(x, xCloseMin, xCloseMax, y, yCloseMin, yCloseMax) && currentCaption is SelectedCaptionButton.Close)
                        {
                            CurrentWindow.Close();
                        }

                        // Title bar drag area
                        else
                        {
                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        currentCaption = SelectedCaptionButton.None;
                        e.Handled = false;
                        break;
                    }
                default:
                    {
                        e.Handled = false;
                        break;
                    }
            }
            if (!IsLeftMouseButtonDown())
            {
                currentCaption = SelectedCaptionButton.None;
            }
        }

        private static bool IsLeftMouseButtonDown()
        {
            // The high-order bit indicates if the key is down
            return (GetAsyncKeyState(VK_LBUTTON) & 0x8000) != 0;
        }

        public void RunMaximization()
        {
            var state = (CurrentWindow.Presenter as OverlappedPresenter).State;
            if (state is OverlappedPresenterState.Restored)
            {
                CurrentWindow.Maximize();
                CheckMaximization();
                return;
            }
            else if (state is OverlappedPresenterState.Maximized)
            {
                CurrentWindow.Restore();
                CheckMaximization();
                return;
            }
        }

        public async void CheckMaximization()
        {
            if (closed) return;

            var _isMaximized = isMaximized;
            if (CurrentWindow.Presenter is OverlappedPresenter presenter)
            {
                switch (presenter.State)
                {
                    case OverlappedPresenterState.Maximized:
                        {
                            await Task.Delay(10);

                            // Required for window drag region
                            additionalHeight = 6;

                            // Required for NCHITTEST
                            isMaximized = true;

                            if (!_isMaximized) SwitchState(ButtonsState.None);

                            break;
                        }
                    case OverlappedPresenterState.Restored:
                        {
                            await Task.Delay(10);

                            // Required for window drag region
                            additionalHeight = 0;

                            // Required for NCHITTEST
                            isMaximized = false;

                            if (_isMaximized) SwitchState(ButtonsState.None);

                            break;
                        }
                }
            }
            else
            {
                await Task.Delay(10);

                // Required for window drag region
                additionalHeight = 0;

                // Required for NCHITTEST
                isMaximized = false;

                return;
            }
        }

        private enum ButtonsState
        {
            None,
            MinimizePointerOver,
            MinimizePressed,
            MaximizePointerOver,
            MaximizePressed,
            ClosePointerOver,
            ClosePressed
        }

        private async void SwitchState(ButtonsState buttonsState)
        {
            CheckMaximization();

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

            _ = VisualStateManager.GoToState(MinimizeButton, minimizeState, true);
            _ = VisualStateManager.GoToState(MaximizeRestoreButton, maximizeState, true);
            _ = VisualStateManager.GoToState(CloseButton, closeState, true);

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

        public async void LoadBounds()
        {
            // Make sure the loop doesn't trigger too often
            await Task.Delay(100);

            try
            {
                // If the window has been closed break the loop
                if (closed) return;

                var collection = GetButtonInteractionArea().Item1;

                // Check if every condition is met
                if (CurrentWindow.AppWindow is not null && IsAutoDragRegionEnabled)
                {
                    // Width (Scaled window width)
                    var width = (int)(CurrentWindow.Bounds.Width * Display.Scale(CurrentWindow));

                    // Height (Scaled control actual height)
                    var height = (int)((ActualHeight + buttonDownHeight) * Display.Scale(CurrentWindow));

                    CurrentWindow.AppWindow.TitleBar.SetDragRectangles([new(0, 0, width, height)]);
                    //CurrentWindow.AppWindow.TitleBar.SetDragRectangles(collection.Concat(new RectInt32(0, 0, width, height).Subtract(GetButtonInteractionArea().Item2)).ToArray());
                }
                else
                {
                    //CurrentWindow.AppWindow.TitleBar.SetDragRectangles(collection);
                }

                LoadBounds();
            }
            catch
            {
                return;
            }
        }

        public (RectInt32[], RectInt32) GetButtonInteractionArea()
        {
            var collection = new RectInt32[0];

            double xMin = 0;
            double yMin = 0;
            double xMax = 0;
            double yMax = 0;

            double xMinimizeMin = 0;
            double xMinimizeMax = 0;
            double xMaximizeMin = 0;
            double xMaximizeMax = 0;
            double xCloseMin = 0;
            double xCloseMax = 0;

            double yMinimizeMin = 0;
            double yMinimizeMax = 0;
            double yMaximizeMin = 0;
            double yMaximizeMax = 0;
            double yCloseMin = 0;
            double yCloseMax = 0;

            try
            {
                if (MinimizeButton != null && MaximizeRestoreButton != null && CloseButton != null)
                {
                    var closeVisual = CloseButton.TransformToVisual(CurrentWindow.Content);
                    var maximizeRestoreVisual = MaximizeRestoreButton.TransformToVisual(CurrentWindow.Content);
                    var minimizeVisual = MinimizeButton.TransformToVisual(CurrentWindow.Content);

                    var minimizeVisualPoint = minimizeVisual.TransformPoint(new Windows.Foundation.Point(0, 0));
                    var maximizeRestoreVisualPoint = maximizeRestoreVisual.TransformPoint(new Windows.Foundation.Point(0, 0));
                    var closeVisualPoint = closeVisual.TransformPoint(new Windows.Foundation.Point(0, 0));

                    // Gets the X positions from: Window X + Window border + (Window size +/- button size)
                    xMinimizeMin =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        minimizeVisualPoint.X * Display.Scale(CurrentWindow);
                    xMinimizeMax =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        (minimizeVisualPoint.X + MinimizeButton.ActualWidth) * Display.Scale(CurrentWindow);
                    xMaximizeMin =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        maximizeRestoreVisualPoint.X * Display.Scale(CurrentWindow);
                    xMaximizeMax =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        (maximizeRestoreVisualPoint.X + MaximizeRestoreButton.ActualWidth) * Display.Scale(CurrentWindow);
                    xCloseMin =
                    CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.X * Display.Scale(CurrentWindow);
                    xCloseMax =
                        CurrentWindow.AppWindow.Position.X +
                        (7 * Display.Scale(CurrentWindow)) +
                        (closeVisualPoint.X + CloseButton.ActualWidth) * Display.Scale(CurrentWindow);

                    // Gets the Y positions from: Window Y + Window border + (Window size +/- button size)
                    yMinimizeMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        minimizeVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yMinimizeMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        minimizeVisualPoint.Y + MinimizeButton.ActualHeight * Display.Scale(CurrentWindow);
                    yMaximizeMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        maximizeRestoreVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yMaximizeMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        maximizeRestoreVisualPoint.Y + MaximizeRestoreButton.ActualHeight * Display.Scale(CurrentWindow);
                    yCloseMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yCloseMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.Y + CloseButton.ActualHeight * Display.Scale(CurrentWindow);
                }
            }
            catch
            {

            }

            if (UseWinUIEverywhere)
            {
                collection = [new RectInt32(
                    (int)(xMaximizeMin * Display.Scale(CurrentWindow)), 
                    (int)(yMaximizeMin * Display.Scale(CurrentWindow)), 
                    (int)((xMaximizeMax + buttonDownHeight) * Display.Scale(CurrentWindow)), 
                    (int)((yMaximizeMax + buttonDownHeight) * Display.Scale(CurrentWindow)))];

                xMin = xMaximizeMin;
                yMin = yMaximizeMin;
                xMax = xMaximizeMax;
                yMax = yMaximizeMax;
            }
            else
            {
                collection = [
                new RectInt32(
                    (int)(xMinimizeMin * Display.Scale(CurrentWindow)),
                    (int)(yMinimizeMin * Display.Scale(CurrentWindow)),
                    (int)((xMinimizeMax + buttonDownHeight) * Display.Scale(CurrentWindow)),
                    (int)((yMinimizeMax + buttonDownHeight) * Display.Scale(CurrentWindow))),
                new RectInt32(
                    (int)(xMaximizeMin * Display.Scale(CurrentWindow)),
                    (int)(yMaximizeMin * Display.Scale(CurrentWindow)),
                    (int)((xMaximizeMax + buttonDownHeight) * Display.Scale(CurrentWindow)),
                    (int)((yMaximizeMax + buttonDownHeight) * Display.Scale(CurrentWindow))),
                new RectInt32(
                    (int)(xCloseMin * Display.Scale(CurrentWindow)),
                    (int)(yCloseMin * Display.Scale(CurrentWindow)),
                    (int)((xCloseMax + buttonDownHeight) * Display.Scale(CurrentWindow)),
                    (int)((yCloseMax + buttonDownHeight) * Display.Scale(CurrentWindow)))];

                xMin = Minimum(xMinimizeMin, xMaximizeMin, xCloseMin);
                yMin = Minimum(yMinimizeMin, yMaximizeMin, yCloseMin);
                xMax = Maximum(xMinimizeMax, xMaximizeMax, xCloseMax);
                yMax = Maximum(yMinimizeMax, yMaximizeMax, yCloseMax);
            }

            return (collection, new RectInt32((int)xMin, (int)yMin, (int)xMax, (int)yMax));
        }

        private double Minimum(params double[] items)
        {
            double min = double.MaxValue;

            foreach (double d in items)
            {
                if (d < min)
                {
                    min = d;
                }
            }

            return min;
        }

        private double Maximum(params double[] items)
        {
            double max = double.MinValue;

            foreach (double d in items)
            {
                if (d > max)
                {
                    max = d;
                }
            }

            return max;
        }
    }
}