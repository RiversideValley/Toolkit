using Riverside.Toolkit.Animations;

namespace Riverside.Toolkit.Controls.Buttons;

public partial class RefreshButton : AnimatedButton
{
    protected bool isRefreshing = false;
    public event EventHandler RefreshClicked;
    public event EventHandler CancelClicked;

    protected UIElement CancelIcon
    {
        get { return (UIElement)GetTemplateChild("CancelIcon"); }
    }

    public RefreshButton()
    {
        this.DefaultStyleKey = typeof(RefreshButton);
        this.AnimatedIcon = new RefreshAnimation();
        this.Click += RefreshButton_Click;
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e) => Refresh();

    protected virtual async void Refresh()
    {
        if (isRefreshing) // Cancel refresh
        {
            CancelIcon.Visibility = Visibility.Collapsed;
            Player.Opacity = 1;
            if (CancelClicked is not null)
                CancelClicked(this, new EventArgs());
            isRefreshing = false;
        }
        else
        {
            if (RefreshClicked is not null)
                RefreshClicked(this, new EventArgs());
            isRefreshing = true;
            await Task.Delay(500); // wait for refresh animation to finish
            if (isRefreshing)
            {
                CancelIcon.Visibility = Visibility.Visible;
                Player.Opacity = 0;
            }
        }
    }
}
