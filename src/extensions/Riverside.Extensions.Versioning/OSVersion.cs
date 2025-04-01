using Riverside.Extensions.Accountability;

namespace Riverside.Extensions.Versioning;

/// <summary>
/// Defines Operating System version
/// </summary>
[NotMyCode("MIT", "https://github.com/CommunityToolkit/WindowsCommunityToolkit", ".NET Foundation", null)]
public struct OSVersion
{
    /// <summary>
    /// Value describing major version
    /// </summary>
    public ushort Major;

    /// <summary>
    /// Value describing minor version
    /// </summary>
    public ushort Minor;

    /// <summary>
    /// Value describing build
    /// </summary>
    public ushort Build;

    /// <summary>
    /// Value describing revision
    /// </summary>
    public ushort Revision;

    /// <summary>
    /// Converts OSVersion to string
    /// </summary>
    /// <returns>Major.Minor.Build.Revision as a string</returns>
    public override readonly string ToString()
        => $"{Major}.{Minor}.{Build}.{Revision}";
}