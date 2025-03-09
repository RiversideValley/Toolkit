using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using WinUIEx;
using WinUIEx.Messaging;
using static Riverside.Toolkit.Helpers.NativeHelper;

namespace Riverside.Toolkit.Controls;

public partial class TitleBarEx : Control
{
    // Check if the cursor's coordinates are inside the specified rect
    public static bool IsInRect(double x, double y, Rect? rect) => rect?.Left <= x && x <= rect?.Right && rect?.Top <= y && y <= rect?.Bottom;

    // Get bounds for a button using visuals
    private Rect? GetButtonBounds(FrameworkElement button, double windowFrameTop)
    {
        // Get visuals
        GeneralTransform visual = button.TransformToVisual(this.CurrentWindow?.Content);
        Point visualPoint = visual.TransformPoint(new Point(0, 0));
        double scale = Display.Scale(this.CurrentWindow);

        if (this.CurrentWindow is null) return null;

        // Create a Rect for the button bounds with scaled positions
        double x = this.CurrentWindow.AppWindow.Position.X + (WND_FRAME_LEFT * scale) + (visualPoint.X * scale);
        double y = this.CurrentWindow.AppWindow.Position.Y + (windowFrameTop * scale) + (visualPoint.Y * scale) + (!isMaximized ? 2 * scale : 0);
        double width = button.ActualWidth * scale;
        double height = button.ActualHeight * scale;

        return new Rect(x, y, width, height);
    }

    // Attach the WndProc onto the window
    private void AttachWndProc()
    {
        if (this.CurrentWindow is not null) messageMonitor ??= new WindowMessageMonitor(this.CurrentWindow);
        if (messageMonitor is null) return;
        messageMonitor.WindowMessageReceived -= WndProc;
        messageMonitor.WindowMessageReceived += WndProc;
    }

    private bool previousButtonDown = false;

    // WndProc for responding to messages with the correct values
    private async void WndProc(object? sender, WindowMessageEventArgs args)
    {
        // If the window is closed stop responding to window messages
        if (closed && messageMonitor is not null)
        {
            messageMonitor.WindowMessageReceived -= WndProc;
            return;
        }

        // Gets the pointer's position relative to the screen's edge with DPI scaling applied
        int x = GetXFromLParam(args.Message.LParam);
        int y = GetYFromLParam(args.Message.LParam);

        double scale = Display.Scale(this.CurrentWindow);

        // Rectangles for button bounds
        Rect? minimizeBounds = new();
        Rect? maximizeBounds = new();
        Rect? closeBounds = new();

        try
        {
            // Retrieve bounds for each button
            if (this.MinimizeButton != null && this.MaximizeRestoreButton != null && this.CloseButton != null)
            {
                minimizeBounds = GetButtonBounds(this.MinimizeButton, additionalHeight);
                maximizeBounds = GetButtonBounds(this.MaximizeRestoreButton, additionalHeight);
                closeBounds = GetButtonBounds(this.CloseButton, additionalHeight);
            }
        }
        catch
        {
            return;
        }

        // If there's no button selected don't extend drag region for checks
        if (this.CurrentCaption is SelectedCaptionButton.None) buttonDownHeight = 0;

        bool IsInMinButton() => IsInRect(x, y, minimizeBounds) && this.MinimizeButton?.Visibility == Visibility.Visible;
        bool IsInMaxButton() => IsInRect(x, y, maximizeBounds) && this.MaximizeRestoreButton?.Visibility == Visibility.Visible;
        bool IsInCloseButton() => IsInRect(x, y, closeBounds);

        switch (args.Message.MessageId)
        {
            // Window activate
            case WM_ACTIVATE:
                {
                    uint wParam = args.Message.WParam.ToUInt32();

                    // Update focus state
                    isWindowFocused = wParam is not WA_INACTIVE;
                    UpdateWindowBrushes();

                    break;
                }

            // Hit test on the non-client area
            case WM_NCHITTEST:
                {
                    // Pointer down hit test
                    if (IsLeftMouseButtonDown())
                    {
                        // Extend drag area
                        buttonDownHeight = 25;

                        InvokeChecks();

                        // Minimize
                        if (IsInMinButton() && this.CurrentCaption is SelectedCaptionButton.Minimize or SelectedCaptionButton.None)
                        {
                            this.CurrentCaption = SelectedCaptionButton.Minimize;

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Minimize, ButtonsState.MinimizePointerOver, ButtonsState.MinimizePressed);

                            args.Handled = true;
                        }

                        // Maximize
                        if (IsInMaxButton() && this.CurrentCaption is SelectedCaptionButton.Maximize or SelectedCaptionButton.None)
                        {
                            this.CurrentCaption = SelectedCaptionButton.Maximize;

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Maximize, ButtonsState.MaximizePointerOver, ButtonsState.MaximizePressed);

                            args.Handled = true;
                        }

                        // Close
                        if (IsInCloseButton() && this.CurrentCaption is SelectedCaptionButton.Close or SelectedCaptionButton.None)
                        {
                            this.CurrentCaption = SelectedCaptionButton.Close;

                            // Update state with the corresponding checks
                            UpdateNonClientHitTestButtonState(SelectedCaptionButton.Close, ButtonsState.ClosePointerOver, ButtonsState.ClosePressed);

                            args.Handled = true;
                        }
                    }

                    // Pointer up hit test
                    else
                    {
                        // Restore drag area
                        buttonDownHeight = 0;

                        InvokeChecks();

                        // Minimize
                        if (IsInMinButton() && this.CurrentCaption != SelectedCaptionButton.Minimize) this.CurrentCaption = SelectedCaptionButton.None;
                        if (IsInMinButton() && this.CurrentCaption == SelectedCaptionButton.Minimize && this.IsMinimizable)
                        {
                            // Minimize
                            _ = VisualStateManager.GoToState(this.MinimizeButton, this.IsMaximizable ? "Normal" : "Disabled", true);
                            await Task.Delay(5);
                            this.CurrentWindow?.Minimize();
                            this.CurrentCaption = SelectedCaptionButton.None;
                            args.Handled = true;
                            return;
                        }

                        // Maximize
                        if (IsInMaxButton() && this.CurrentCaption != SelectedCaptionButton.Maximize) this.CurrentCaption = SelectedCaptionButton.None;
                        if (IsInMaxButton() && this.CurrentCaption == SelectedCaptionButton.Maximize && this.IsMaximizable)
                        {
                            // Maximize
                            CheckMaximization();
                            if (isMaximized) this.CurrentWindow?.Restore();
                            else this.CurrentWindow?.Maximize();
                            this.CurrentCaption = SelectedCaptionButton.None;
                            args.Handled = true;
                            return;
                        }

                        // Close
                        if (IsInCloseButton() && this.CurrentCaption != SelectedCaptionButton.Close) this.CurrentCaption = SelectedCaptionButton.None;
                        if (IsInCloseButton() && this.CurrentCaption == SelectedCaptionButton.Close && this.IsClosable)
                        {
                            // Close
                            this.CurrentCaption = SelectedCaptionButton.None;
                            args.Handled = true;
                            this.CurrentWindow?.Close();
                        }

                        // Handle hit test
                        args.Handled = true;

                        // Logic for pressing the left mouse button, moving the pointer to a different button, and releasing it there
                        if (this.CurrentCaption == SelectedCaptionButton.None && previousButtonDown == true)
                        {
                            // Update state
                            previousButtonDown = IsLeftMouseButtonDown();

                            // Prevent action
                            return;
                        }
                    }

                    // Update state
                    previousButtonDown = IsLeftMouseButtonDown();

                    // Minimize Button
                    if (IsInMinButton())
                    {
                        // If the button is disabled return border
                        if (!this.IsMinimizable)
                        {
                            CancelHitTest();
                            return;
                        }

                        // Update state with the corresponding checks
                        UpdateNonClientHitTestButtonState(SelectedCaptionButton.Minimize, ButtonsState.MinimizePointerOver, ButtonsState.MinimizePressed);

                        // Check if the current caption is correct
                        if (this.CurrentCaption is SelectedCaptionButton.None)
                        {
                            RespondToHitTest(HTMINBUTTON);
                        }
                        return;
                    }

                    // Maximize Button
                    else if (IsInRect(x, y, maximizeBounds) && this.MaximizeRestoreButton?.Visibility == Visibility.Visible)
                    {
                        // If the button is disabled return border
                        if (!this.IsMaximizable)
                        {
                            CancelHitTest();
                            return;
                        }

                        // Update state with the corresponding checks
                        UpdateNonClientHitTestButtonState(SelectedCaptionButton.Maximize, ButtonsState.MaximizePointerOver, ButtonsState.MaximizePressed);

                        // Check if the current caption is correct
                        if (this.CurrentCaption is SelectedCaptionButton.None)
                        {
                            RespondToHitTest(HTMAXBUTTON);
                        }
                        return;
                    }

                    // Close Button
                    else if (IsInRect(x, y, closeBounds))
                    {
                        // If the button is disabled return border
                        if (!this.IsClosable)
                        {
                            CancelHitTest();
                            return;
                        }

                        // Update state with the corresponding checks
                        UpdateNonClientHitTestButtonState(SelectedCaptionButton.Close, ButtonsState.ClosePointerOver, ButtonsState.ClosePressed);

                        // Check if the current caption is correct
                        if (this.CurrentCaption is SelectedCaptionButton.None)
                        {
                            RespondToHitTest(HTCLOSE);
                        }
                        return;
                    }

                    SwitchState(ButtonsState.None);

                    args.Handled = false;

                    return;
                }

            // Left-click down on the non-client area
            case WM_NCLBUTTONDOWN:
                {
                    args.Handled = true;

                    buttonDownHeight = 25;

                    // Minimize Button
                    if (IsInRect(x, y, minimizeBounds) && this.MinimizeButton?.Visibility == Visibility.Visible)
                    {
                        // If the button is disabled return border
                        if (!this.IsMinimizable) CancelButtonDown();

                        // Update selected button
                        this.CurrentCaption = SelectedCaptionButton.Minimize;

                        // Update state with the corresponding checks
                        UpdateNonClientHitTestButtonState(SelectedCaptionButton.Minimize, ButtonsState.MinimizePointerOver, ButtonsState.MinimizePressed);

                        return;
                    }

                    // Maximize Button
                    else if (IsInRect(x, y, maximizeBounds) && this.MaximizeRestoreButton?.Visibility == Visibility.Visible)
                    {
                        // If the button is disabled return border
                        if (!this.IsMaximizable) CancelButtonDown();

                        // Update selected button
                        this.CurrentCaption = SelectedCaptionButton.Maximize;

                        // Update state with the corresponding checks
                        UpdateNonClientHitTestButtonState(SelectedCaptionButton.Maximize, ButtonsState.MaximizePointerOver, ButtonsState.MaximizePressed);
                    }

                    // Close Button
                    else if (IsInRect(x, y, closeBounds))
                    {
                        // If the button is disabled return border
                        if (!this.IsClosable) CancelButtonDown();

                        // Update selected button
                        this.CurrentCaption = SelectedCaptionButton.Close;

                        // Update state with the corresponding checks
                        UpdateNonClientHitTestButtonState(SelectedCaptionButton.Close, ButtonsState.ClosePointerOver, ButtonsState.ClosePressed);
                    }

                    // Title bar drag area
                    else
                    {
                        this.CurrentCaption = SelectedCaptionButton.None;

                        args.Handled = false;

                        // No buttons are selected
                        SwitchState(ButtonsState.None);
                    }

                    break;
                }

            // Right-click on the non-client area
            case WM_NCRBUTTONUP:
                {
                    // Show custom right-click menu if not using WinUI everywhere or clicking on a button
                    if (!this.UseWinUIEverywhere || IsInRect(x, y, minimizeBounds) || IsInRect(x, y, maximizeBounds) || IsInRect(x, y, closeBounds) || this.CurrentWindow is null)
                        return;

                    args.Handled = true;

                    // Show the custom right-click flyout at the appropriate position
                    this.CustomRightClickFlyout?.ShowAt(this, new Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowOptions()
                    {
                        Position = new Point(x - this.CurrentWindow.AppWindow.Position.X - (WND_FRAME_LEFT * scale),
                                             y - this.CurrentWindow.AppWindow.Position.Y - (additionalHeight * scale))
                    });

                    break;
                }

            // Double-click on the non-client area (title bar)
            case WM_NCLBUTTONDBLCLK:
                {
                    // Prevent action if the window is maximizable
                    args.Handled = !this.IsMaximizable;
                    break;
                }

            // Left-click up on the non-client area
            case WM_NCLBUTTONUP:
                {
                    // Reset the button states when no buttons are selected
                    SwitchState(ButtonsState.None);
                    this.CurrentCaption = SelectedCaptionButton.None;
                    break;
                }
        }

        void UpdateNonClientHitTestButtonState(SelectedCaptionButton button, ButtonsState pointerOver, ButtonsState pressed) => SwitchState(
                // If the current caption is none, select it as usual
                this.CurrentCaption == SelectedCaptionButton.None ? pointerOver : // False state

                // If the current caption is the button's type, select the button as pressed
                this.CurrentCaption == button ? pressed : // False state

                // Otherwise, this is not the button the user previously selected
                ButtonsState.None);

        void CancelButtonDown()
        {
            // Cancel every other button
            SwitchState(ButtonsState.None);

            args.Handled = false;
            return;
        }

        void CancelHitTest()
        {
            // Cancel every other button
            SwitchState(ButtonsState.None);

            RespondToHitTest(HTBORDER);
        }

        void RespondToHitTest(int hitTest) => args.Result = new IntPtr(hitTest);
    }
}
