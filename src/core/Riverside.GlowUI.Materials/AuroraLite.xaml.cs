// Don't worry about the AuroraLite.xaml file not being in the shared project, it's inside the target-specific projects :)

namespace Riverside.GlowUI.Materials;

// [XamlLocationAttribute(CodeLocation.TargetSpecificProjects)]
public sealed partial class AuroraLite : UserControl
{
    public AuroraLite()
    {
        this.InitializeComponent();
    }
}
