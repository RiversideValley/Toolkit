namespace Riverside.Toolkit.Converters;

/// <summary>
/// Converts an enum value to a boolean and vice versa.
/// </summary>
/// <typeparam name="TEnum">The enum type.</typeparam>
public partial class EnumToBoolConverter<TEnum> : ValueConverter<TEnum, bool> where TEnum : struct, Enum
{
    /// <summary>
    /// Converts an enum value to a boolean.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <param name="parameter">An optional parameter.</param>
    /// <param name="language">The language information.</param>
    /// <returns>A boolean indicating if the enum value is equal to 1.</returns>
    protected override bool Convert(TEnum value, object? parameter, string? language)
        => System.Convert.ToInt32(value) == 1;

    /// <summary>
    /// Converts a boolean back to the enum value.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <param name="parameter">An optional parameter.</param>
    /// <param name="language">The language information.</param>
    /// <returns>The enum value corresponding to the boolean.</returns>
    protected override TEnum ConvertBack(bool value, object? parameter, string? language)
        => (TEnum)Enum.ToObject(typeof(TEnum), value == true ? 1 : 0);
}
