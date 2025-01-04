using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;

namespace Riverside.Toolkit.Controls
{
    /// <summary>
    /// Represents a custom CommandLink control.
    /// </summary>
    public sealed class CommandLink : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLink"/> class.
        /// </summary>
        public CommandLink()
        {
            this.DefaultStyleKey = typeof(CommandLink);
            this.PointerEntered += CommandLink_PointerEntered;
            this.PointerPressed += CommandLink_PointerPressed;
            this.PointerReleased += CommandLink_PointerReleased;
            this.PointerExited += CommandLink_PointerExited;
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
        "Title", // The name of the property
        typeof(string), // The type of the property
        typeof(CommandLink), // The type of the owner class
        new PropertyMetadata("Title") // Default value
        );

        /// <summary>
        /// Gets or sets the title of the CommandLink.
        /// </summary>
        [Browsable(true)]
        [Category("Common")]
        [Description("The title of the CommandLink")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Description"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
        "Description", // The name of the property
        typeof(string), // The type of the property
        typeof(CommandLink), // The type of the owner class
        new PropertyMetadata("Description") // Default value
        );

        /// <summary>
        /// Gets or sets the description of the CommandLink.
        /// </summary>
        [Browsable(true)]
        [Category("Common")]
        [Description("The description of the CommandLink")]
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        private bool invokedFromLeftButton;

        /// <summary>
        /// Handles the PointerExited event of the CommandLink control.
        /// Changes the visual state to "Normal".
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void CommandLink_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        /// <summary>
        /// Handles the PointerReleased event of the CommandLink control.
        /// Changes the visual state to "PointerOver" and invokes the Click event if the left button was pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void CommandLink_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
            if (invokedFromLeftButton == true) this.Click?.Invoke(this, new RoutedEventArgs());
        }

        /// <summary>
        /// Handles the PointerPressed event of the CommandLink control.
        /// Changes the visual state to "Pressed" if the left button is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void CommandLink_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
                invokedFromLeftButton = true;
            }
            else invokedFromLeftButton = false;
        }

        /// <summary>
        /// Handles the PointerEntered event of the CommandLink control.
        /// Changes the visual state to "PointerOver" or "Pressed" based on the pointer state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
        private void CommandLink_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false) VisualStateManager.GoToState(this, "PointerOver", true);
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true) VisualStateManager.GoToState(this, "Pressed", true);
        }

        /// <summary>
        /// Occurs when the CommandLink is clicked.
        /// </summary>
        public event RoutedEventHandler Click;
    }
}
