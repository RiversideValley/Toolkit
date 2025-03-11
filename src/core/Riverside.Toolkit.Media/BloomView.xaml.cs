namespace Riverside.Toolkit.Media;

public sealed partial class BloomView : UserControl
{
    public BloomView()
    {
        this.InitializeComponent();
        BloomWebView.Height = Window.Current.Bounds.Height;
        BloomWebView.Width = Window.Current.Bounds.Width;
#if Uwp
        BloomWebView.Source = new("ms-appx-web:///Riverside.Toolkit.Uwp.Media/Bloom.html");
#elif WinUI
        BloomWebView.Source = new("ms-appx-web:///Riverside.Toolkit.WinUI.Media/Bloom.html");
#endif
#if WinUI
        BloomWebView.DefaultBackgroundColor = Colors.Transparent;
#endif
    }

    private void Bloom_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        BloomWebView.Height = Window.Current.Bounds.Height;
        BloomWebView.Width = Window.Current.Bounds.Width;
    }
}