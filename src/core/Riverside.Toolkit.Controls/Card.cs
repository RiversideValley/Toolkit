namespace Riverside.Toolkit.Controls;

/// <summary>
/// Represents a custom Card control.
/// </summary>
public sealed partial class Card : Control
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Card"/> class.
    /// </summary>
    public Card()
    {
        this.DefaultStyleKey = typeof(Card);
        PointerEntered += Card_PointerEntered;
        PointerPressed += Card_PointerPressed;
        PointerReleased += Card_PointerReleased;
        PointerExited += Card_PointerExited;
    }

    /// <summary>
    /// Identifies the <see cref="Title"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register(
    "Title", // The name of the property
    typeof(string), // The type of the property
    typeof(Card), // The type of the owner class
    new PropertyMetadata("Title") // Default value
    );

    /// <summary>
    /// Gets or sets the title of the Card.
    /// </summary>
    [Browsable(true)]
    [Category("Common")]
    [Description("The title of the Card")]
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Subtitle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SubtitleProperty =
    DependencyProperty.Register(
    "Subtitle", // The name of the property
    typeof(string), // The type of the property
    typeof(Card), // The type of the owner class
    new PropertyMetadata("Subtitle") // Default value
    );

    /// <summary>
    /// Gets or sets the subtitle of the Card.
    /// </summary>
    [Browsable(true)]
    [Category("Common")]
    [Description("The Subtitle of the Card")]
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="Content"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
    DependencyProperty.Register(
    "Content", // The name of the property
    typeof(string), // The type of the property
    typeof(Card), // The type of the owner class
    new PropertyMetadata("Content") // Default value
    );

    /// <summary>
    /// Gets or sets the content of the Card.
    /// </summary>
    [Browsable(true)]
    [Category("Common")]
    [Description("The Content of the Card")]
    public string Content
    {
        get => (string)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    private bool invokedFromLeftButton;

    /// <summary>
    /// Handles the PointerExited event of the Card control.
    /// Changes the visual state to "Normal".
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
    private void Card_PointerExited(object sender, PointerRoutedEventArgs e) => VisualStateManager.GoToState(this, "Normal", true);

    /// <summary>
    /// Handles the PointerReleased event of the Card control.
    /// Changes the visual state to "PointerOver" and invokes the Click event if the left button was pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
    private void Card_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _ = VisualStateManager.GoToState(this, "PointerOver", true);
        if (invokedFromLeftButton == true) Click?.Invoke(this, new RoutedEventArgs());
    }

    /// <summary>
    /// Handles the PointerPressed event of the Card control.
    /// Changes the visual state to "Pressed" if the left button is pressed.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
    private void Card_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true)
        {
            _ = VisualStateManager.GoToState(this, "Pressed", true);
            invokedFromLeftButton = true;
        }
        else
        {
            invokedFromLeftButton = false;
        }
    }

    /// <summary>
    /// Handles the PointerEntered event of the Card control.
    /// Changes the visual state to "PointerOver" or "Pressed" based on the pointer state.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="PointerRoutedEventArgs"/> instance containing the event data.</param>
    private void Card_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == false) _ = VisualStateManager.GoToState(this, "PointerOver", true);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed == true) _ = VisualStateManager.GoToState(this, "Pressed", true);
    }

    /// <summary>
    /// Occurs when the Card is clicked.
    /// </summary>
    public event RoutedEventHandler Click;
}
