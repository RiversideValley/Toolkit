namespace Riverside.Toolkit.Controls;

public sealed partial class GlowBall : UserControl
{
    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color), typeof(UserControl), null);

    public GlowBall()
    {
        this.InitializeComponent();
    }
}