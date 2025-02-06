namespace Riverside.Toolkit.Converters;

/// <summary>
/// A value converter that converts null values to true and non-null values to false.
/// </summary>
public partial class NullToTrueConverter : ValueConverter<object?, bool>
{
    /// <summary>
    /// Determines whether an inverse conversion should take place.
    /// </summary>
    /// <remarks>If set, the value <see langword="true"/> results in <see cref="Visibility.Collapsed"/>, and <see langword="false"/> in <see cref="Visibility.Visible"/>.</remarks>
    public bool Inverse { get; set; }

    /// <summary>
    /// Converts a source value to the target type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override bool Convert(object? value, object? parameter, string? language)
        => this.Inverse ? value != null : value == null;

    /// <summary>
    /// Converts a target value back to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override object? ConvertBack(bool value, object? parameter, string? language)
        => null;
}