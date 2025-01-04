using WinUIEx;
using CommunityToolkit.WinUI.UI.Helpers;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        private void SwitchButtonStatePointerEvent(object sender, PointerRoutedEventArgs e)
        {
            SwitchState(ButtonsState.None);
            InvokeChecks();
        }

        private void Content_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            SwitchState(ButtonsState.None);
            InvokeChecks();
        }

        private void ContentLoaded(object sender, RoutedEventArgs e) => InvokeChecks();

        private void CurrentWindow_WindowStateChanged(object? sender, WindowState e) => InvokeChecks();

        private void CurrentWindow_PositionChanged(object? sender, Windows.Graphics.PointInt32 e) => InvokeChecks();

        private void CurrentWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args) => InvokeChecks();

        private void ThemeListener_ThemeChanged(ThemeListener sender) => InvokeChecks();

        private void CurrentWindow_Closed(object sender, WindowEventArgs args)
        {
            args.Handled = !IsClosable;
            closed = IsClosable;
        }

        private void CheckMouseButtonDownPointerEvent(object sender, PointerRoutedEventArgs e)
        {
            SwitchState(ButtonsState.None);

            if (!IsLeftMouseButtonDown())
                CurrentCaption = SelectedCaptionButton.None;
        }
    }
}
