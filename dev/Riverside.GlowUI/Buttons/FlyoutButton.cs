using CommunityToolkit.Mvvm.ComponentModel;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace Cube.UI.Buttons;

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
