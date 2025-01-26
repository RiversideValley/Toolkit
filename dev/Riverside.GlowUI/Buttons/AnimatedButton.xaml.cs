using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace Cube.UI.Buttons;

[INotifyPropertyChanged]
public partial class AnimatedButton : Button
{
    private IAnimatedVisualSource icon;

    public IAnimatedVisualSource AnimatedIcon
    {
        get => icon;
        set
        {
            SetProperty(ref icon, value);
            if (Player is not null)
                Player.Source = value;
        }
    }

    protected AnimatedVisualPlayer Player
    {
        get { return (AnimatedVisualPlayer)GetTemplateChild("Icon"); }
    }

    public AnimatedButton() => this.DefaultStyleKey = typeof(AnimatedButton);

    protected async override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        this.Click += async delegate { await Player.PlayAsync(0, 1, false); };
        if (AnimatedIcon is not null)
            Player.Source = AnimatedIcon;
    }
}
