using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Riverside.Toolkit.Controls
{
    public sealed class Card : Control
    {
        public Card()
        {
            this.DefaultStyleKey = typeof(Card);
            this.PointerEntered += Card_PointerEntered;
            this.PointerPressed += Card_PointerPressed;
            this.PointerReleased += Card_PointerReleased;
            this.PointerExited += Card_PointerExited;
        }

        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
        "Title", // The name of the property
        typeof(string), // The type of the property
        typeof(Card), // The type of the owner class
        new PropertyMetadata("Title") // Default value
        );

        [Browsable(true)]
        [Category("Common")]
        [Description("The title of the Card")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(
        "Subtitle", // The name of the property
        typeof(string), // The type of the property
        typeof(Card), // The type of the owner class
        new PropertyMetadata("Subtitle") // Default value
        );

        [Browsable(true)]
        [Category("Common")]
        [Description("The Subtitle of the Card")]
        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(
        "Content", // The name of the property
        typeof(string), // The type of the property
        typeof(Card), // The type of the owner class
        new PropertyMetadata("Content") // Default value
        );

        [Browsable(true)]
        [Category("Common")]
        [Description("The Content of the Card")]
        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private bool invokedFromLeftButton;

        private void Card_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        private void Card_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
            if (invokedFromLeftButton == true) this.Click?.Invoke(this, new RoutedEventArgs());
        }

        private void Card_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
                invokedFromLeftButton = true;
            }
            else invokedFromLeftButton = false;
        }

        private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false) VisualStateManager.GoToState(this, "PointerOver", true);
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true) VisualStateManager.GoToState(this, "Pressed", true);
        }

        public event RoutedEventHandler Click;
    }
}