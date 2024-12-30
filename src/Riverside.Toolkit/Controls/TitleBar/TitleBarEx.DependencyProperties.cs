using DependencyPropertyGenerator;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Riverside.Toolkit.Controls.TitleBar
{
    [DependencyProperty<bool>("IsAutoDragRegionEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsAccentTitleBarEnabled", DefaultValue = true)]
    [DependencyProperty<bool>("IsMinimizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsMaximizable", DefaultValue = true)]
    [DependencyProperty<bool>("IsClosable", DefaultValue = true)]
    [DependencyProperty<bool>("UseWinUIEverywhere", DefaultValue = false)]
    [DependencyProperty<bool>("MemorizeWindowPosition", DefaultValue = false)]
    [DependencyProperty<Color>("CaptionForegroundInteract")]
    [DependencyProperty<SolidColorBrush>("CurrentForeground")]
    [DependencyProperty<string>("Title", DefaultValue = "Window Title")]
    [DependencyProperty<string>("Subtitle")]
    [DependencyProperty<string>("WindowTag", DefaultValue = "Main")]
    public partial class TitleBarEx
    {
    }
}
