using Windows.System.Profile;
using Riverside.Toolkit.Brushes;
using CommunityToolkit.WinUI.Helpers;

namespace Riverside.GlowUI.Materials;

[Obsolete("This class is not currently available as it is based on TenMica, which is a Windows Metadata component, which is not supported in UWP .NET 9. The control has been disabled for now and will be re-implemented in a future release.")]
public sealed partial class Mica10DeepBackground : Page
{
    public Mica10DeepBackground()
    {
        this.InitializeComponent();
        string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
        ulong version = ulong.Parse(deviceFamilyVersion);
        ulong build = (version & 0x00000000FFFF0000L) >> 16;

        if (build >= 22000)
        {
            MicaDeepLayer.Visibility = Visibility.Visible;
            TenMicaLayer.Visibility = Visibility.Collapsed;

            var m = new MicaAltBrush();
            m.Kind = (int)BackdropKind.BaseAlt;
            m.Theme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Default;
            this.Background = m;
        }
        else
        {
            MicaDeepLayer.Visibility = Visibility.Collapsed;
            TenMicaLayer.Visibility = Visibility.Visible;
        }

        var Listener = new ThemeListener();
        Listener.ThemeChanged += Listener_ThemeChanged;
    }

    private void Listener_ThemeChanged(ThemeListener sender)
    {
        string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
        ulong version = ulong.Parse(deviceFamilyVersion);
        ulong build = (version & 0x00000000FFFF0000L) >> 16;

        if (build >= 22000)
        {
            MicaDeepLayer.Visibility = Visibility.Visible;
            TenMicaLayer.Visibility = Visibility.Collapsed;

            var m = new MicaAltBrush();
            m.Kind = (int)BackdropKind.BaseAlt;
            m.Theme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Default;
            this.Background = m;
        }
    }
}
