using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        protected void SwitchState(ButtonsState buttonsState)
        {
            // Required for maximize button states
            CheckMaximization();

            // If the buttons don't exist return
            if (CloseButton is null || MaximizeRestoreButton is null || MinimizeButton is null) return;

            // Default states
            string minimizeState = IsMinimizable ? "Normal" : "Disabled";
            string maximizeState = IsMaximizable ? isMaximized ? "Checked" : "Normal" : isMaximized ? "CheckedDisabled" : "Disabled";
            string closeState = IsClosable ? "Normal" : "Disabled";

            // Switch based on button states
            switch (buttonsState)
            {
                // Minimize button
                case ButtonsState.MinimizePointerOver or ButtonsState.MinimizePressed:
                    {
                        minimizeState =
                            // Check if button action is allowed
                            IsMinimizable ?

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
                            IsMaximizable ?

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
                            IsClosable ?

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

            CheckMaximizeNormalStates();

            // Handle WinUI tooltips
            if (UseWinUIEverywhere) HandleTooltips(buttonsState);

            // Apply the visual states based on the calculated states
            _ = VisualStateManager.GoToState(MinimizeButton, minimizeState, true);
            _ = VisualStateManager.GoToState(MaximizeRestoreButton, maximizeState, true);
            _ = VisualStateManager.GoToState(CloseButton, closeState, true);

            // Local method to check the maximize state
            void CheckMaximizeNormalStates()
            {
                if (buttonsState is not (ButtonsState.MaximizePointerOver or ButtonsState.MaximizePressed))
                    maximizeState =
                        // Is maximizable
                        IsMaximizable ? isMaximized ? "Checked" : "Normal" :

                        // Is not maximizable
                        isMaximized ? "CheckedDisabled" : "Disabled";
            }

            // Local method to handle tooltips for WinUI
            void HandleTooltips(ButtonsState buttonsState)
            {
                var minimizeTooltip = ToolTipService.GetToolTip(MinimizeButton) as ToolTip;
                var closeTooltip = ToolTipService.GetToolTip(CloseButton) as ToolTip;

                minimizeTooltip.IsOpen = buttonsState == ButtonsState.MinimizePointerOver;
                closeTooltip.IsOpen = buttonsState == ButtonsState.ClosePointerOver;
            }
        }
    }
}