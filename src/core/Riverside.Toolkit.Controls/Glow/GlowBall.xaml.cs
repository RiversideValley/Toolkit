// Don't worry about the GlowBall.xaml file not being in the shared project, it's inside the target-specific projects :)

namespace Riverside.Toolkit.Controls.Glow;

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