using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Riverside.Toolkit.Controls
{
    public sealed class CommandLink : Control
    {
        public CommandLink()
        {
            this.DefaultStyleKey = typeof(CommandLink);
            this.PointerEntered += CommandLink_PointerEntered;
            this.PointerPressed += CommandLink_PointerPressed;
            this.PointerReleased += CommandLink_PointerReleased;
            this.PointerExited += CommandLink_PointerExited;
        }

        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
        "Title", // The name of the property
        typeof(string), // The type of the property
        typeof(CommandLink), // The type of the owner class
        new PropertyMetadata("Title") // Default value
        );

        [Browsable(true)]
        [Category("Common")]
        [Description("The title of the CommandLink")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
        "Description", // The name of the property
        typeof(string), // The type of the property
        typeof(CommandLink), // The type of the owner class
        new PropertyMetadata("Description") // Default value
        );

        [Browsable(true)]
        [Category("Common")]
        [Description("The description of the CommandLink")]
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        private bool invokedFromLeftButton;

        private void CommandLink_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void CommandLink_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
            if (invokedFromLeftButton == true) this.Click?.Invoke(this, new RoutedEventArgs());
        }

        private void CommandLink_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
                invokedFromLeftButton = true;
            }
            else invokedFromLeftButton = false;
        }

        private void CommandLink_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false) VisualStateManager.GoToState(this, "PointerOver", true);
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true) VisualStateManager.GoToState(this, "Pressed", true);
        }

        public event RoutedEventHandler Click;
    }
}