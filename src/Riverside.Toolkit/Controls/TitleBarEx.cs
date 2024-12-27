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

namespace Riverside.Toolkit.Controls
{
    [DependencyProperty<bool>("IsAutoDragRegionEnabled", DefaultValue = true)]
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
        private Border AccentStrip { get; set; }
        private WindowMessageMonitor MessageMonitor { get; set; }
        private bool isWindowFocused { get; set; } = false;

        private double additionalHeight { get; set; } = 0;

        private SelectedCaptionButton currentCaption { get; set; } = SelectedCaptionButton.None;

        private bool closed;

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

        public void InitializeForWindow(WindowEx windowEx)
        {
            CurrentWindow = windowEx;

            CurrentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            CurrentWindow.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;

            CurrentWindow.Content.PointerMoved += PointerMoved;
            CurrentWindow.Content.PointerReleased += PointerReleased;
            CurrentWindow.Content.PointerExited += PointerExited;

            CurrentWindow.WindowStateChanged += CurrentWindow_WindowStateChanged;

            CurrentWindow.Closed += CurrentWindow_Closed;

            CheckWindow();
            Rehook();
            LoadBounds();
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
                /*if (WindowTitle != null && CurrentWindow != null)
                {
                    WindowTitle.Text = CurrentWindow.Title;
                }*/
                await Task.Delay(50);
                CheckFocus();
                CheckWindow();
            }
            catch
            {

            }
        }

        private void CurrentWindow_WindowStateChanged(object sender, WindowState e) => CheckMaximization();

        private void PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(CurrentWindow.Content).Properties.IsLeftButtonPressed != true)
            {
                currentCaption = SelectedCaptionButton.None;
            }
        }

        private void PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(CurrentWindow.Content).Properties.IsLeftButtonPressed != true)
            {
                currentCaption = SelectedCaptionButton.None;
            }
        }

        private void PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                if (CurrentWindow != null)
                {
                    if (CurrentWindow.Content != null)
                    {
                        if (e.GetCurrentPoint(CurrentWindow.Content).Properties.IsLeftButtonPressed != true)
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
                if (TitleBarIcon is not null) TitleBarIcon.Source = new BitmapImage(new Uri($"ms-appx:///{path}"));
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
            if (IsAccentColorEnabledForTitleBars() == true)
            {
                try
                {
                    if (AccentStrip != null)
                    {
                        AccentStrip.Visibility = isWindowFocused == false ? Visibility.Visible : Visibility.Collapsed;
                    }
                    Resources["CaptionForegroundBrush"] = isWindowFocused == false ? new SolidColorBrush(Colors.White) : Application.Current.Resources["AccentTextFillColorDisabledBrush"] as SolidColorBrush;
                }
                catch { }
            }
            else
            {
                try
                {
                    if (AccentStrip != null)
                    {
                        AccentStrip.Visibility = Visibility.Collapsed;
                    }
                    Resources["CaptionForegroundBrush"] = isWindowFocused == false ? Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush : Application.Current.Resources["TextFillColorDisabledBrush"] as SolidColorBrush;
                }
                catch { }
            }
            UpdateBrush();
        }

        private async void Event(object sender, WindowMessageEventArgs e)
        {
            if (closed) return;

            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int WM_NCHITTEST = 0x0084;
            const int WM_NCLBUTTONUP = 0x00A2;
            const int WM_NCMOUSELEAVE = 0x02A2;
            const int WM_ACTIVATE = 0x0006;
            const int WA_INACTIVE = 0;

            // Gets the pointer's position relative to the screen's edge with DPI scaling applied
            var x = GET_X_LPARAM(e.Message.LParam);
            var y = GET_Y_LPARAM(e.Message.LParam);

            double xMinimizeMin = 0;
            double xMinimizeMax = 0;

            double xMaximizeMin = 0;
            double xMaximizeMax = 0;

            double xCloseMin = 0;
            double xCloseMax = 0;

            double yMin = 0;
            double yMax = 0;

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
                    yMin =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.Y + 2 * Display.Scale(CurrentWindow);

                    yMax =
                        CurrentWindow.AppWindow.Position.Y +
                        (additionalHeight * Display.Scale(CurrentWindow)) +
                        closeVisualPoint.Y + CloseButton.ActualHeight + 2 * Display.Scale(CurrentWindow);
                }
            }
            catch
            {
                return;
            }

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
                        // Minimize Button
                        if (IsInRect(x, xMinimizeMin, xMinimizeMax, y, yMin, yMax))
                        {
                            e.Handled = true;
                            e.Result = new IntPtr(8);
                            _ = currentCaption == SelectedCaptionButton.Minimize
                                ? VisualStateManager.GoToState(MinimizeButton, "Pressed", true)
                                : currentCaption == SelectedCaptionButton.None
                                    ? VisualStateManager.GoToState(MinimizeButton, "PointerOver", true)
                                    : VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                            await Task.Delay(1000);
                            e.Handled = false;
                        }

                        // Maximize Button
                        else if (IsInRect(x, xMaximizeMin, xMaximizeMax, y, yMin, yMax))
                        {
                            e.Handled = true;
                            e.Result = new IntPtr(9);
                            _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = currentCaption == SelectedCaptionButton.Maximize
                                ? VisualStateManager.GoToState(MaximizeRestoreButton, "Pressed", true)
                                : currentCaption == SelectedCaptionButton.None
                                    ? VisualStateManager.GoToState(MaximizeRestoreButton, "PointerOver", true)
                                    : VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                            await Task.Delay(1000);
                            e.Handled = false;
                        }

                        // Close Button
                        else if (IsInRect(x, xCloseMin, xCloseMax, y, yMin, yMax))
                        {
                            e.Handled = true;
                            e.Result = new IntPtr(20);
                            _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = currentCaption == SelectedCaptionButton.Close
                                ? VisualStateManager.GoToState(CloseButton, "Pressed", true)
                                : currentCaption == SelectedCaptionButton.None
                                    ? VisualStateManager.GoToState(CloseButton, "PointerOver", true)
                                    : VisualStateManager.GoToState(CloseButton, "Normal", true);
                            await Task.Delay(1000);
                            e.Handled = false;
                        }

                        // Title bar drag area
                        else
                        {
                            e.Handled = true;
                            e.Result = new IntPtr(1);
                            _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                            e.Handled = false;
                        }

                        break;
                    }
                case WM_NCLBUTTONDOWN:
                    {
                        e.Handled = true;
                        e.Result = new IntPtr(1);
                        e.Handled = false;

                        // Minimize Button
                        if (IsInRect(x, xMinimizeMin, xMinimizeMax, y, yMin, yMax))
                        {
                            currentCaption = SelectedCaptionButton.Minimize;
                            _ = VisualStateManager.GoToState(MinimizeButton, "Pressed", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                        }

                        // Maximize Button
                        else if (IsInRect(x, xMaximizeMin, xMaximizeMax, y, yMin, yMax))
                        {
                            currentCaption = SelectedCaptionButton.Maximize;
                            _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Pressed", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                        }

                        // Close Button
                        else if (IsInRect(x, xCloseMin, xCloseMax, y, yMin, yMax))
                        {
                            currentCaption = SelectedCaptionButton.Close;
                            _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Pressed", true);
                        }

                        // Title bar drag area
                        else
                        {
                            currentCaption = SelectedCaptionButton.None;
                            _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                            _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                            _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                        }

                        break;
                    }
                case WM_NCLBUTTONUP:
                    {
                        e.Handled = true;
                        e.Result = new IntPtr(1);
                        e.Handled = false;

                        // Minimize Button
                        if (IsInRect(x, xMinimizeMin, xMinimizeMax, y, yMin, yMax))
                        {
                            if (currentCaption == SelectedCaptionButton.Minimize)
                            {
                                CurrentWindow.Minimize();
                                _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                                _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                                _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                            }
                        }

                        // Maximize Button
                        else if (IsInRect(x, xMaximizeMin, xMaximizeMax, y, yMin, yMax))
                        {
                            if (currentCaption == SelectedCaptionButton.Maximize)
                            {
                                RunMaximization();
                            }
                        }

                        // Close Button
                        else if (IsInRect(x, xCloseMin, xCloseMax, y, yMin, yMax))
                        {
                            if (currentCaption == SelectedCaptionButton.Close)
                            {
                                CurrentWindow.Close();
                            }
                        }

                        // Title bar drag area
                        else
                        {

                        }

                        currentCaption = SelectedCaptionButton.None;

                        MessageMonitor.WindowMessageReceived -= Event;
                        MessageMonitor.Dispose();
                        break;
                    }
                case WM_NCMOUSELEAVE:
                    {
                        e.Handled = true;
                        e.Result = new IntPtr(1);
                        e.Handled = false;
                        _ = VisualStateManager.GoToState(MinimizeButton, "Normal", true);
                        _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                        _ = VisualStateManager.GoToState(CloseButton, "Normal", true);
                        break;
                    }
                default:
                    {
                        e.Handled = false;
                        break;
                    }
            }
        }

        public void RunMaximization()
        {
            var state = (CurrentWindow.Presenter as OverlappedPresenter).State;
            if (state == OverlappedPresenterState.Restored)
            {
                CurrentWindow.Maximize();
                CheckMaximization();
                return;
            }
            else if (state == OverlappedPresenterState.Maximized)
            {
                CurrentWindow.Restore();
                CheckMaximization();
                return;
            }
        }

        public async void CheckMaximization()
        {
            if (CurrentWindow.Presenter is OverlappedPresenter presenter)
            {
                var state = presenter.State;
                if (state is OverlappedPresenterState.Restored)
                {
                    await Task.Delay(10);
                    _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);

                    // Required for window drag region
                    additionalHeight = 0;

                    return;
                }
                else if (state is OverlappedPresenterState.Maximized)
                {
                    await Task.Delay(10);
                    _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Checked", true);

                    // Required for window drag region
                    additionalHeight = 6;

                    return;
                }
            }
            else
            {
                await Task.Delay(10);
                _ = VisualStateManager.GoToState(MaximizeRestoreButton, "Normal", true);
                return;
            }
        }

        private void UpdateBrush()
        {
            try
            {
                if (CloseButton != null && MaximizeRestoreButton != null && MinimizeButton != null && TitleTextBlock != null)
                {
                    CloseButton.Foreground = Resources["CaptionForegroundBrush"] as SolidColorBrush;
                    MaximizeRestoreButton.Foreground = Resources["CaptionForegroundBrush"] as SolidColorBrush;
                    MinimizeButton.Foreground = Resources["CaptionForegroundBrush"] as SolidColorBrush;
                    TitleTextBlock.Foreground = Resources["CaptionForegroundBrush"] as SolidColorBrush;
                }
            }
            catch
            {

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

                // Check if every condition is met
                if (CurrentWindow.AppWindow is not null && IsAutoDragRegionEnabled)
                {
                    // Width (Scaled window width)
                    var width = (int)(CurrentWindow.Bounds.Width * Display.Scale(CurrentWindow));

                    // Height (Scaled control actual height)
                    var height = (int)(ActualHeight * Display.Scale(CurrentWindow));

                    CurrentWindow.AppWindow.TitleBar.SetDragRectangles([new RectInt32(0, 0, width, height)]);
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