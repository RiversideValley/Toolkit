using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using WinUIEx;

#nullable enable

namespace Riverside.Toolkit.Controls.TitleBar
{
    public partial class TitleBarEx
    {
        private void SwitchButtonStatePointerEvent(object sender, PointerRoutedEventArgs e) => SwitchState(ButtonsState.None);

        private void Content_PointerEntered(object sender, PointerRoutedEventArgs e) => SwitchState(ButtonsState.None);

        private async void CurrentWindow_WindowStateChanged(object? sender, WindowState e) => await CheckMaximization();

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
