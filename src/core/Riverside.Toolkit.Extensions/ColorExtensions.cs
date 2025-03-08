namespace Riverside.Toolkit.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Color"/> <see langword="struct"/>.
/// </summary>
[NotMyCode("MIT", "http://github.com/validvoid/TextBlockFX", "Void", "2021")]
public static class ColorExtensions
{
    /// <summary>
    /// Returns a new <see cref="Color"/> with the specified alpha value.
    /// </summary>
    /// <param name="color">The original color.</param>
    /// <param name="alpha">The alpha value to set, between <c>0.0</c> and <c>1.0</c>.</param>
    /// <returns>A new <see cref="Color"/> with the specified alpha value.</returns>
    public static Color WithAlpha(this Color color, double alpha)
    {
        double safeAlpha = Math.Max(Math.Min(alpha, 1.0), 0.0);
        byte alphaByte = (byte)(safeAlpha * 255);
        return Color.FromArgb(alphaByte, color.R, color.G, color.B);
    }
}
