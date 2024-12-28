using WinUIEx;

namespace Riverside.Toolkit.UITests
{
    public sealed partial class UnitTestAppWindow : WindowEx
    {
        public UnitTestAppWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            TitleBar.InitializeForWindow(this);
            TitleBar.SetWindowIcon(@"Assets/Rebound.ico");
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }
    }
}