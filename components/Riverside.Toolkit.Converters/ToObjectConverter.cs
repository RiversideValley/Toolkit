namespace Riverside.Toolkit.Converters;

/// <summary>
/// The base class for converting instances of type T to object and vice versa.
/// </summary>
public abstract class ToObjectConverter<T> : ValueConverter<T?, object?>
{
    /// <summary>
    /// Converts a source value to the target type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override object? Convert(T? value, object? parameter, string? language) => value;

    /// <summary>
    /// Converts a target value back to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override T? ConvertBack(object? value, object? parameter, string? language) => (T?)value;
}