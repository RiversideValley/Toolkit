using CommunityToolkit.Mvvm.ComponentModel;

namespace Riverside.Toolkit.Controls.Buttons;

[INotifyPropertyChanged]
public partial class FlyoutButton : ToggleButton
{
    private FlyoutBase flyout;

    public FlyoutBase Flyout
    {
        get => flyout;
        set
        {
            SetProperty(ref flyout, value);
            value.ShouldConstrainToRootBounds = false;
            value.Closed += delegate { this.IsChecked = false; };
            value.Opened += delegate { this.IsChecked = true; };
        }
    }

    public FlyoutButton()
    {
        this.DefaultStyleKey = typeof(FlyoutButton);
        this.Click += delegate { Flyout.ShowAt(this); };
    }
}
