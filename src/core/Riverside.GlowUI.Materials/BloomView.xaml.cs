// Don't worry about the BloomView.xaml file not being in the shared project, it's inside the target-specific projects :)

namespace Riverside.GlowUI.Materials;

public sealed partial class BloomView : UserControl
{
    public BloomView()
    {
        this.InitializeComponent();
        BloomWebView.Height = Window.Current.Bounds.Height;
        BloomWebView.Width = Window.Current.Bounds.Width;
    }

    private void Bloom_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        BloomWebView.Height = Window.Current.Bounds.Height;
        BloomWebView.Width = Window.Current.Bounds.Width;
    }
}