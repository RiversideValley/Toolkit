using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIEx.Messaging;
using static Riverside.Toolkit.Helpers.NativeHelper;

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx : Control
    {
        // Window messages

        // Non client left button down
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        // Non client hit test
        private const int WM_NCHITTEST = 0x0084;
        // Non client left button up
        private const int WM_NCLBUTTONUP = 0x00A2;
        // Non client right button up
        private const int WM_NCRBUTTONUP = 0x00A5;
        // Activate
        private const int WM_ACTIVATE = 0x0006;

        // NCHITTEST response

        // Hit test minimize button
        private const int HTMINBUTTON = 8;
        // Hit test maximize button
        private const int HTMAXBUTTON = 9;
        // Hit test close button
        private const int HTCLOSE = 20;
        // Hit test border (for no interaction)
        private const int HTBORDER = 18;
        // Title bar
        private const int HTCAPTION = 2;

        // Window frame

        // Window frame width
        private const int WND_FRAME_LEFT = 7;
        // Top window frame (it goes up by 6px when maximized because of how Windows works)
        // 6 pixels instead of 7 because one is still there for some reason (the rest are above the pixel grid when the window is maximized)
        private const int WND_FRAME_TOP_MAXIMIZED = 7;
        // Top window frame (not maximized)
        private const int WND_FRAME_TOP_NORMAL = 1;

        // Others

        // Activate (inactive)
        private const int WA_INACTIVE = 0;

        public static bool IsInRect(double x, double y, Rect rect) => rect.Left <= x && x <= rect.Right && rect.Top <= y && y <= rect.Bottom;

        private Rect GetButtonBounds(FrameworkElement button, double windowFrameTop)
        {
            // Get visuals
            var visual = button.TransformToVisual(CurrentWindow.Content);
            var visualPoint = visual.TransformPoint(new Point(0, 0));
            double scale = Display.Scale(CurrentWindow);

            // Create a Rect for the button bounds with scaled positions
            double x = CurrentWindow.AppWindow.Position.X + (WND_FRAME_LEFT * scale) + visualPoint.X * scale;
            double y = CurrentWindow.AppWindow.Position.Y + (windowFrameTop * scale) + visualPoint.Y * scale + (!isMaximized ? 2 * scale : 0);
            double width = button.ActualWidth * scale;
            double height = button.ActualHeight * scale;

            return new Rect(x, y, width, height);
        }

        // WndProc for responding to messages with the correct values
        private async void WndProc(object sender, WindowMessageEventArgs e)
        {
            // If the window is closed stop responding to window messages
            if (closed) return;

            // Gets the pointer's position relative to the screen's edge with DPI scaling applied
            var x = GetXFromLParam(e.Message.LParam);
            var y = GetYFromLParam(e.Message.LParam);

            double scale = Display.Scale(CurrentWindow);

            // Rectangles for button bounds
            Rect minimizeBounds = new();
            Rect maximizeBounds = new();
            Rect closeBounds = new();

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
            if (currentCaption is SelectedCaptionButton.None) buttonDownHeight = 0;

            switch (e.Message.MessageId)
            {
                case WM_ACTIVATE:
                    {
                        var wParam = e.Message.WParam.ToUInt32();

                        // Update focus state
                        isWindowFocused = wParam is not WA_INACTIVE;
                        CheckFocus();

                        break;
                    }
                case WM_NCHITTEST:
                    {
                        e.Handled = true;

                        if (IsLeftMouseButtonDown()) buttonDownHeight = 25; 
                        else buttonDownHeight = 0;

                        // Minimize Button
                        if (IsInRect(x, y, minimizeBounds) && MinimizeButton.Visibility == Visibility.Visible)
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
                            if (currentCaption is SelectedCaptionButton.None)
                            {
                                await RespondToHitTest(HTMINBUTTON);
                            }
                        }

                        // Maximize Button
                        else if (IsInRect(x, y, maximizeBounds) && MaximizeRestoreButton.Visibility == Visibility.Visible)
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
                            if (currentCaption is SelectedCaptionButton.None)
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
                            if (currentCaption is SelectedCaptionButton.None)
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
                case WM_NCLBUTTONDOWN:
                    {
                        e.Handled = true;

                        if (IsLeftMouseButtonDown()) buttonDownHeight = 25;
                        else buttonDownHeight = 0;

                        // Minimize Button
                        if (IsInRect(x, y, minimizeBounds) && IsLeftMouseButtonDown() && MinimizeButton.Visibility == Visibility.Visible)
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
                        else if (IsInRect(x, y, maximizeBounds) && MaximizeRestoreButton.Visibility == Visibility.Visible)
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
                        else if (IsInRect(x, y, closeBounds))
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
                case WM_NCRBUTTONUP:
                    {
                        if (!UseWinUIEverywhere || IsInRect(x, y, minimizeBounds) || IsInRect(x, y, maximizeBounds) || IsInRect(x, y, closeBounds)) return;

                        e.Handled = true;

                        CustomRightClickFlyout.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions()
                        {
                            Position = new Point(x - CurrentWindow.AppWindow.Position.X - WND_FRAME_LEFT * scale, y - CurrentWindow.AppWindow.Position.Y - additionalHeight * scale)
                        });

                        break;
                    }
                case WM_NCLBUTTONUP:
                    {
                        e.Handled = true;
                        e.Result = new IntPtr(1);
                        // Minimize Button
                        if (IsInRect(x, y, minimizeBounds) && currentCaption is SelectedCaptionButton.Minimize)
                        {
                            if (!IsMinimizable)
                            {
                                e.Handled = false;
                                return;
                            }

                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        // Maximize Button
                        else if (IsInRect(x, y, maximizeBounds) && currentCaption is SelectedCaptionButton.Maximize)
                        {
                            if (!IsMaximizable)
                            {
                                e.Handled = false;
                                return;
                            }

                            // No buttons are selected
                            SwitchState(ButtonsState.None);
                        }

                        // Close Button
                        else if (IsInRect(x, y, closeBounds) && currentCaption is SelectedCaptionButton.Close)
                        {
                            if (!IsClosable)
                            {
                                e.Handled = false;
                                return;
                            }
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

            // Check if the left mouse button has been released in the meantime
            currentCaption = IsLeftMouseButtonDown() ? currentCaption : SelectedCaptionButton.None;

            void UpdateNonClientHitTestButtonState(SelectedCaptionButton button, ButtonsState pointerOver, ButtonsState pressed)
            {
                SwitchState(
                    // If the current caption is none, select it as usual
                    currentCaption == SelectedCaptionButton.None ? pointerOver : // False state

                    // If the current caption is the button's type, select the button as pressed
                    currentCaption == button ? pressed : // False state

                    // Otherwise, this is not the button the user previously selected
                    ButtonsState.None);
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