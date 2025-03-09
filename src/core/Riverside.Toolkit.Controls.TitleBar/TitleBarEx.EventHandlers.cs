using WinUIEx;

namespace Riverside.Toolkit.Controls.TitleBar;

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

    private void CurrentWindow_Closed(object sender, WindowEventArgs args)
    {
        args.Handled = !this.IsClosable;
        closed = this.IsClosable;
    }

    private void CheckMouseButtonDownPointerEvent(object sender, PointerRoutedEventArgs e)
    {
        SwitchState(ButtonsState.None);

        if (!IsLeftMouseButtonDown())
            this.CurrentCaption = SelectedCaptionButton.None;
    }
}
