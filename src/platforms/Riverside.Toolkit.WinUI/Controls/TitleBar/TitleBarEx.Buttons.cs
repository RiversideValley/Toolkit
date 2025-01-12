#if WinUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar;

public partial class TitleBarEx
{
    protected void SwitchState(ButtonsState buttonsState)
    {
        // If the buttons don't exist return
        if (this.CloseButton is null || this.MaximizeRestoreButton is null || this.MinimizeButton is null || closed) return;

        // Default states
        string minimizeState = this.IsMinimizable ? "Normal" : "Disabled";
        string maximizeState = this.IsMaximizable ? isMaximized ? "Checked" : "Normal" : isMaximized ? "CheckedDisabled" : "Disabled";
        string closeState = this.IsClosable ? "Normal" : "Disabled";

        // Switch based on button states
        switch (buttonsState)
        {
            // Minimize button
            case ButtonsState.MinimizePointerOver or ButtonsState.MinimizePressed:
                {
                    minimizeState =
                        // Check if button action is allowed
                        this.IsMinimizable ?

                        // Button action is allowed
                        (buttonsState == ButtonsState.MinimizePointerOver ?

                        // Pointer is over
                        "PointerOver" :

                        // Pointer is pressed
                        "Pressed") :

                        // Button action is not allowed
                        "Disabled";

                    break;
                }

            // Maximize button
            case ButtonsState.MaximizePointerOver or ButtonsState.MaximizePressed:
                {
                    maximizeState =
                        // Check if button action is allowed
                        this.IsMaximizable ?

                        // Button action is allowed
                        buttonsState == ButtonsState.MaximizePointerOver

                        // Pointer is over
                        ? (isMaximized ? "CheckedPointerOver" : "PointerOver")

                        // Pointer is pressed
                        : (isMaximized ? "CheckedPressed" : "Pressed") :

                        // Button action is not allowed
                        isMaximized ? "CheckedDisabled" : "Disabled";

                    break;
                }

            // Close button
            case ButtonsState.ClosePointerOver or ButtonsState.ClosePressed:
                {
                    closeState =
                        // Check if button action is allowed
                        this.IsClosable ?

                        // Button action is allowed
                        (buttonsState == ButtonsState.ClosePointerOver ?

                        // Pointer is over
                        "PointerOver" :

                        // Pointer is pressed
                        "Pressed") :

                        // Button action is not allowed
                        "Disabled";

                    break;
                }
        }

        // Check the maximize state separately
        if (buttonsState is not (ButtonsState.MaximizePointerOver or ButtonsState.MaximizePressed))
        {
            maximizeState =
                // Is maximizable
                this.IsMaximizable ? isMaximized ? "Checked" : "Normal" :

                // Is not maximizable
                isMaximized ? "CheckedDisabled" : "Disabled";
        }

        // Handle WinUI tooltips
        if (this.UseWinUIEverywhere)
        {
            var minimizeTooltip = (ToolTip)ToolTipService.GetToolTip(this.MinimizeButton);
            var closeTooltip = (ToolTip)ToolTipService.GetToolTip(this.CloseButton);

            minimizeTooltip.IsOpen = buttonsState == ButtonsState.MinimizePointerOver;
            closeTooltip.IsOpen = buttonsState == ButtonsState.ClosePointerOver;
        }

        // Apply the visual states based on the calculated states
        _ = VisualStateManager.GoToState(this.MinimizeButton, minimizeState, true);
        _ = VisualStateManager.GoToState(this.MaximizeRestoreButton, maximizeState, true);
        _ = VisualStateManager.GoToState(this.CloseButton, closeState, true);
    }
}
#endif