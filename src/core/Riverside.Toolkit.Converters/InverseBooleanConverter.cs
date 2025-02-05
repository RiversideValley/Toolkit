namespace Riverside.Toolkit.Converters;

/// <summary>
/// Converts a boolean to and from a visibility value.
/// </summary>
public partial class InverseBooleanConverter
    : ValueConverter<bool, bool>
{
    /// <summary>
    /// Converts a source value to the target type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override bool Convert(bool value, object? parameter, string? language) => !value;

    /// <summary>
    /// Converts a target value back to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override bool ConvertBack(bool value, object? parameter, string? language) => !value;
}