namespace Riverside.Toolkit.Controls
{
    /// <summary>
    /// Represents a custom ChatBubble control.
    /// </summary>
    public sealed class ChatBubble : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatBubble"/> class.
        /// </summary>
        public ChatBubble()
        {
            this.DefaultStyleKey = typeof(ChatBubble);
        }

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
        "Text", // The name of the property
        typeof(string), // The type of the property
        typeof(ChatBubble), // The type of the owner class
        new PropertyMetadata("Text") // Default value
        );

        /// <summary>
        /// Gets or sets the text in the ChatBubble.
        /// </summary>
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
