namespace Riverside.Toolkit.Converters;

/// <summary>
/// Converts a boolean value to a Visibility value.
/// </summary>
public partial class BooleanToVisibilityConverter : IValueConverter
{
#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
    /// <summary>
    /// Converts a <see langword="bool"/> value to a <see cref="Visibility"/> value.
    /// </summary>
    /// <param name="value">The <see langword="bool"/> value to convert.</param>
    /// <returns><see cref="Visibility.Visible"/> if the value is <see langword="true"/>; otherwise, <see cref="Visibility.Collapsed"/>.</returns>
    public object Convert(object value, Type targetType, object parameter, string language) => (bool)value ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Converts a <see cref="Visibility"/> value back to a <see langword="bool"/> value.
    /// </summary>
    /// <param name="value">The <see cref="Visibility"/> value to convert.</param>
    /// <returns><see langword="true"/> if the value is <see cref="Visibility.Visible"/>; otherwise, <see langword="false"/>.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language) => (Visibility)value == Visibility.Visible;
#pragma warning restore CS1573
}
