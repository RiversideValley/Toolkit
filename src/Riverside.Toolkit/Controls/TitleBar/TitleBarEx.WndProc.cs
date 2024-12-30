using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIEx.Messaging;
using static Riverside.Toolkit.Helpers.NativeHelper;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx : Control
    {
        // Check if the cursor's coordinates are inside the specified rect
        public static bool IsInRect(double x, double y, Rect? rect) => rect?.Left <= x && x <= rect?.Right && rect?.Top <= y && y <= rect?.Bottom;

        // Get bounds for a button using visuals
        private Rect? GetButtonBounds(FrameworkElement button, double windowFrameTop)
        {
            // Get visuals
            var visual = button.TransformToVisual(CurrentWindow?.Content);
            var visualPoint = visual.TransformPoint(new Point(0, 0));
            double scale = Display.Scale(CurrentWindow);

            if (CurrentWindow is null) return null;

            // Create a Rect for the button bounds with scaled positions
            double x = CurrentWindow.AppWindow.Position.X + (WND_FRAME_LEFT * scale) + visualPoint.X * scale;
            double y = CurrentWindow.AppWindow.Position.Y + (windowFrameTop * scale) + visualPoint.Y * scale + (!isMaximized ? 2 * scale : 0);
            double width = button.ActualWidth * scale;
            double height = button.ActualHeight * scale;

            return new Rect(x, y, width, height);
        }

        // Attach the WndProc onto the window
        private void AttachWndProc()
        {
            if (CurrentWindow is not null) messageMonitor ??= new WindowMessageMonitor(CurrentWindow);
            if (messageMonitor is null) return;
            messageMonitor.WindowMessageReceived -= WndProc;
            messageMonitor.WindowMessageReceived += WndProc;
        }

        // WndProc for responding to messages with the correct values
        private async void WndProc(object? sender, WindowMessageEventArgs e)
        {
            // If the window is closed stop responding to window messages
            if (closed && messageMonitor is not null)
            {
                messageMonitor.WindowMessageReceived -= WndProc;
                return;
            }

            // Gets the pointer's position relative to the screen's edge with DPI scaling applied
            var x = GetXFromLParam(e.Message.LParam);
            var y = GetYFromLParam(e.Message.LParam);

            double scale = Display.Scale(CurrentWindow);

            // Rectangles for button bounds
            Rect? minimizeBounds = new();
            Rect? maximizeBounds = new();
            Rect? closeBounds = new();

            try
            {
                // Retrieve bounds for each button
                if (MinimizeButton != null && MaximizeRestoreButton != null && CloseButton != null)
                {
                    minimizeBounds = GetButtonBounds(MinimizeButton, additionalHeight);
                    maximizeBounds = GetButtonBounds(MaximizeRestoreButton, additionalHeight);
                    closeBounds = GetButtonBounds(CloseButton, additionalHeight);
                }
            }
            catch
            {
                return;
            }

            // If there's no button selected don't extend drag region for checks
            if (CurrentCaption is SelectedCaptionButton.None) buttonDownHeight = 0;

            switch (e.Message.MessageId)
            {
                // Window activate
                case WM_ACTIVATE:
                    {
                        var wParam = e.Message.WParam.ToUInt32();

                        // Update focus state
                        isWindowFocused = wParam is not WA_INACTIVE;
                        UpdateWindowBrushes();

                        break;
                    }

                // Hit test on the non-client area
                case WM_NCHITTEST:
                    {
                        e.Handled = true;

                        if (IsLeftMouseButtonDown()) buttonDownHeight = 25; 
                        else buttonDownHeight = 0;

                        // Minimize Button
                        if (IsInRect(x, y, minimizeBounds) && MinimizeButton?.Visibility == Visibility.Visible)
                        {
                            // If the button is disabled return border
                            if (!IsMinimizable)
                            {
                                await CancelHitTest();
                                return;
                            }

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Minimize, ButtonsState.MinimizePointerOver, ButtonsState.MinimizePressed);

                            // Check if the current caption is correct
                            if (CurrentCaption is SelectedCaptionButton.None)
                            {
                                await RespondToHitTest(HTMINBUTTON);
                            }
                        }

                        // Maximize Button
                        else if (IsInRect(x, y, maximizeBounds) && MaximizeRestoreButton?.Visibility == Visibility.Visible)
                        {
                            // If the button is disabled return border
                            if (!IsMaximizable)
                            {
                                await CancelHitTest();
                                return;
                            }

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Maximize, ButtonsState.MaximizePointerOver, ButtonsState.MaximizePressed);

                            // Check if the current caption is correct
                            if (CurrentCaption is SelectedCaptionButton.None)
                            {
                                await RespondToHitTest(HTMAXBUTTON);
                            }
                        }

                        // Close Button
                        else if (IsInRect(x, y, closeBounds))
                        {
                            // If the button is disabled return border
                            if (!IsClosable)
                            {
                                await CancelHitTest();
                                return;
                            }

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Close, ButtonsState.ClosePointerOver, ButtonsState.ClosePressed);

                            // Check if the current caption is correct
                            if (CurrentCaption is SelectedCaptionButton.None)
                            {
                                await RespondToHitTest(HTCLOSE);
                            }
                        }

                        // Title bar drag area
                        else
                        {
                            e.Handled = false;
                            return;
                        }

                        e.Handled = false;

                        break;
                    }

                // Left-click down on the non-client area
                case WM_NCLBUTTONDOWN:
                    {
                        e.Handled = true;

                        if (IsLeftMouseButtonDown()) buttonDownHeight = 25;
                        else buttonDownHeight = 0;

                        // Minimize Button
                        if (IsInRect(x, y, minimizeBounds) && MinimizeButton?.Visibility == Visibility.Visible)
                        {
                            // If the button is disabled return border
                            if (!IsMinimizable) CancelButtonDown();

                            // Update selected button
                            CurrentCaption = SelectedCaptionButton.Minimize;

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Minimize, ButtonsState.MinimizePointerOver, ButtonsState.MinimizePressed);
                        }

                        // Maximize Button
                        else if (IsInRect(x, y, maximizeBounds) && MaximizeRestoreButton?.Visibility == Visibility.Visible)
                        {
                            // If the button is disabled return border
                            if (!IsMaximizable) CancelButtonDown();

                            // Update selected button
                            CurrentCaption = SelectedCaptionButton.Maximize;

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Maximize, ButtonsState.MaximizePointerOver, ButtonsState.MaximizePressed);
                        }

                        // Close Button
                        else if (IsInRect(x, y, closeBounds))
                        {
                            // If the button is disabled return border
                            if (!IsClosable) CancelButtonDown();

                            // Update selected button
                            CurrentCaption = SelectedCaptionButton.Close;

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Close, ButtonsState.ClosePointerOver, ButtonsState.ClosePressed);
                        }

                        // Title bar drag area
                        else
                        {
                            CurrentCaption = SelectedCaptionButton.None;

                            e.Handled = false;

                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        break;
                    }

                // Right-click on the non-client area
                case WM_NCRBUTTONUP:
                    {
                        // Show custom right-click menu if not using WinUI everywhere or clicking on a button
                        if (!UseWinUIEverywhere || IsInRect(x, y, minimizeBounds) || IsInRect(x, y, maximizeBounds) || IsInRect(x, y, closeBounds) || CurrentWindow is null)
                            return;

                        e.Handled = true;

                        // Show the custom right-click flyout at the appropriate position
                        CustomRightClickFlyout?.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions()
                        {
                            Position = new Point(x - CurrentWindow.AppWindow.Position.X - WND_FRAME_LEFT * scale,
                                                 y - CurrentWindow.AppWindow.Position.Y - additionalHeight * scale)
                        });

                        break;
                    }

                // Double-click on the non-client area (title bar)
                case WM_NCLBUTTONDBLCLK:
                    {
                        // Prevent action if the window is maximizable
                        e.Handled = !IsMaximizable;
                        break;
                    }

                // Left-click up on the non-client area
                case WM_NCLBUTTONUP:
                    {
                        // Reset the button states when no buttons are selected
                        SwitchState(ButtonsState.None);
                        CurrentCaption = SelectedCaptionButton.None;
                        break;
                    }
            }

            // Check if the left mouse button has been released in the meantime
            CurrentCaption = IsLeftMouseButtonDown() ? CurrentCaption : SelectedCaptionButton.None;

            void UpdateNonClientHitTestButtonState(SelectedCaptionButton button, ButtonsState pointerOver, ButtonsState pressed)
            {
                SwitchState(
                    // If the current caption is none, select it as usual
                    CurrentCaption == SelectedCaptionButton.None ? pointerOver : // False state

                    // If the current caption is the button's type, select the button as pressed
                    CurrentCaption == button ? pressed : // False state

                    // Otherwise, this is not the button the user previously selected
                    ButtonsState.None);
            }

            void CancelButtonDown()
            {
                // Cancel every other button
                SwitchState(ButtonsState.None);

                e.Handled = false;
                return;
            }

            async Task CancelHitTest()
            {
                // Cancel every other button
                SwitchState(ButtonsState.None);

                await RespondToHitTest(HTBORDER);
            }

            async Task RespondToHitTest(int hitTest)
            {
                e.Result = new IntPtr(hitTest);
                await Task.Delay(800);
                e.Handled = false;
            }
        }
    }
}