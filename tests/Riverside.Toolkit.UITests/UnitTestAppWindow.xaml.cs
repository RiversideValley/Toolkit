using Microsoft.UI.Xaml;
using WinUIEx;

namespace Riverside.Toolkit.UITests
{
    public sealed partial class UnitTestAppWindow : WindowEx
    {
        public UnitTestAppWindow()
        {
            InitializeComponent();
            TitleBar.InitializeForWindow(this, Application.Current);
            TitleBar.SetWindowIcon(@"Assets/Rebound.ico");
            SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }
    }
}