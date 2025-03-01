namespace Riverside.Extensions.Accountability;

/// <summary>
/// This attribute is used to indicate that the code is not owned by you.
/// It can be consumed by tools to ensure that the code is used legally.
/// You should use it when copying code from the internet or other sources.
/// </summary>
/// <remarks>
/// This attribute does not have any effect on the code itself, and is used for metadata and accountability.
/// </remarks>
[AttributeUsage(AttributeTargets.All)]
public class NotMyCodeAttribute : Attribute
{
    /// <summary>
    /// Indicates that the code is not owned by you.
    /// </summary>
    /// <remarks>
    /// This is a generic indication, it is recommended that you use a more specific attribute if possible.
    /// </remarks>
    /// <example>
    /// [NotMyCode()]
    /// public struct HashCode
    /// </example>
    /// <seealso cref="NotMyCodeAttribute(Uri)"/>
    /// <seealso cref="NotMyCodeAttribute(string)"/>
    /// <seealso cref="NotMyCodeAttribute(string, string)"/>
    public NotMyCodeAttribute()
    {
        License = null;
        Permission = "This code is used legally under the terms of the original license.";
    }

    /// <summary>
    /// Indicates that the code is not owned by you.
    /// </summary>
    /// <param name="license">The SPDX expression of the license that the code associated with this attribute is licensed under.</param>
    /// <example>
    /// [NotMyCode("MIT")]
    /// public struct HashCode
    /// </example>
    public NotMyCodeAttribute(string license)
    {
        License = license;
        Permission = "This code is used legally as it is licensed under the " + license + " license.";
    }

    /// <summary>
    /// Indicates that the code is not owned by you.
    /// </summary>
    /// <param name="license">A URI that points to the license file that the code associated with this attribute is licensed under.</param>
    /// <example>
    /// [NotMyCode(new("https://github.com/dotnet/runtime/blob/main/LICENSE.TXT"))]
    /// public struct HashCode
    /// </example>
    public NotMyCodeAttribute(Uri license)
    {
        License = license;
        Permission = "This code is used legally as the original author of this software has granted permission to use this software at " + license.ToString();
    }

    /// <summary>
    /// Indicates that the code is not owned by you.
    /// </summary>
    /// <param name="permissionMessage">The written permission the author of this software gave you to use the code associated with this attribute.</param>
    /// <param name="license">The SPDX expression of the license that the code associated with this attribute is licensed under.</param>
    /// <example>
    /// [NotMyCode(".NET (including the runtime repo) is licensed under the MIT license.")]
    /// public struct HashCode
    /// </example>
    /// <example>
    /// [NotMyCode(".NET (including the runtime repo) is licensed under the MIT license.", "MIT")]
    /// public struct HashCode
    /// </example>
    public NotMyCodeAttribute(string permissionMessage, string? license)
    {
        License = license;
        Permission = permissionMessage;
    }

    /// <summary>
    /// The permission message that the author of this software gave you to use the code associated with this attribute.
    /// </summary>
    public string Permission;

    /// <summary>
    /// The license that the code associated with this attribute is licensed under.
    /// </summary>
    public object? License;
}
