using WinUIEx;

namespace Riverside.Toolkit.UITests
{
    public sealed partial class UnitTestAppWindow : WindowEx
    {
        public UnitTestAppWindow()
        {
            InitializeComponent();
            TitleBar.InitializeForWindow(this);
            TitleBar.SetWindowIcon("Assets/Rebound.ico");
        }
    }
}