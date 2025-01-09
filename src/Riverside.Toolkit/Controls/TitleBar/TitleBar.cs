namespace Riverside.Toolkit.Controls.TitleBar;

/// <summary>
/// Specifies the selected caption button.
/// </summary>
public enum SelectedCaptionButton
{
    /// <summary>
    /// No button is selected.
    /// </summary>
    None = 0,
    /// <summary>
    /// The minimize button is selected.
    /// </summary>
    Minimize = 1,
    /// <summary>
    /// The maximize button is selected.
    /// </summary>
    Maximize = 2,
    /// <summary>
    /// The close button is selected.
    /// </summary>
    Close = 3
}

/// <summary>
/// Specifies the state of the caption buttons.
/// </summary>
public enum ButtonsState
{
    /// <summary>
    /// No button state.
    /// </summary>
    None,
    /// <summary>
    /// The minimize button is in the pointer over state.
    /// </summary>
    MinimizePointerOver,
    /// <summary>
    /// The minimize button is in the pressed state.
    /// </summary>
    MinimizePressed,
    /// <summary>
    /// The maximize button is in the pointer over state.
    /// </summary>
    MaximizePointerOver,
    /// <summary>
    /// The maximize button is in the pressed state.
    /// </summary>
    MaximizePressed,
    /// <summary>
    /// The close button is in the pointer over state.
    /// </summary>
    ClosePointerOver,
    /// <summary>
    /// The close button is in the pressed state.
    /// </summary>
    ClosePressed
}
