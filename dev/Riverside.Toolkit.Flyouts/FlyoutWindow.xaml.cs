using H.NotifyIcon.Core;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Riverside.Toolkit.Flyouts.Helpers;
using Riverside.Toolkit.Flyouts.Interfaces;
using WinUIEx;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Riverside.Toolkit.Flyouts
{
    /// <summary>
    /// A flyout window
    /// </summary>
    public partial class FlyoutWindow : BaseWindow, IFlyoutWindow
    {
        public IPositionHelper FlyoutPositionHelper = new PositionHelper();


        public FlyoutWindow()
        {
            FlyoutPositionHelper.Positionflyout(this);
            this.InitializeComponent();
            this.SetTitleBarBackgroundColors(Colors.Transparent);
            this.Activated += Flyout_Activated;
            this.Show();
            this.BringToFront();
            this.SetForegroundWindow();
            //  Icon.Create();
            // Icon.MessageWindow.MouseEventReceived += MessageWindow_MouseEventReceived;
        }

        private void MessageWindow_MouseEventReceived(object sender, MessageWindow.MouseEventReceivedEventArgs e)
        {
            if (e.MouseEvent is MouseEvent.IconLeftMouseUp)
            {
                this.Show();
                this.BringToFront();
                this.SetForegroundWindow();
                FlyoutBase.ShowAttachedFlyout(FlyoutGrid);
            }
        }

        private void Flyout_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
        {
            //  if (args.WindowActivationState == WindowActivationState.Deactivated)
            // this.Hide();
        }

        private void Flyout_Closed(object sender, object e)
        {
            FlyoutBase.ShowAttachedFlyout(FlyoutGrid);
            // this.Hide();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(FlyoutGrid);
        }
    }
}
