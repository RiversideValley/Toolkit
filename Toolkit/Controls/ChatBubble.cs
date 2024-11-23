using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Riverside.Toolkit.Controls
{
    public sealed class ChatBubble : Control
    {
        public ChatBubble()
        {
            this.DefaultStyleKey = typeof(ChatBubble);
        }

        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
        "Text", // The name of the property
        typeof(string), // The type of the property
        typeof(ChatBubble), // The type of the owner class
        new PropertyMetadata("Text") // Default value
        );

        [Browsable(true)]
        [Category("Common")]
        [Description("The text in the ChatBubble")]
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }
    }
}
