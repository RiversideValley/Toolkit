namespace Riverside.Toolkit.Converters;

public partial class StringNullOrWhiteSpaceToTrueConverter
    : ValueConverter<string, bool>
{
    /// <summary>
    /// Determines whether an inverse conversion should take place.
    /// </summary>
    /// <remarks>If set, the value True results in <see cref="Visibility.Collapsed"/>, and false in <see cref="Visibility.Visible"/>.</remarks>
    public bool Inverse { get; set; }

    /// <summary>
    /// Converts a source value to the target type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override bool Convert(string? value, object? parameter, string? language) => this.Inverse ? !string.IsNullOrWhiteSpace(value) : string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Converts a target value back to the source type.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parameter"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    protected override string ConvertBack(bool value, object? parameter, string? language) => string.Empty;
}
