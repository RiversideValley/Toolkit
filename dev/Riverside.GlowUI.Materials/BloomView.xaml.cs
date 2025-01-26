using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Riverside.GlowUI.Materials
{
    public sealed partial class BloomView : UserControl
    {
        public BloomView()
        {
            this.InitializeComponent();
            BloomWebView.Height = this.Height;
            BloomWebView.Width = this.Width;
        }

        private void Bloom_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BloomWebView.Height = e.NewSize.Height;
            BloomWebView.Width = e.NewSize.Width;
        }
    }
}
