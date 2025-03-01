using LicenseIdentifiers;

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
    public NotMyCodeAttribute()
    {
        License = null;
        Permission = "This code is used legally under the terms of the original license.";
    }

    /// <summary>
    /// Indicates that the code is not owned by you.
    /// </summary>
    /// <param name="license">The <see cref="LicenseIdentifier"/> of the license that the code associated with this attribute is licensed under.</param>
    /// <example>
    /// [NotMyCode(LicenseIdentifier.MIT)]
    /// public struct HashCode
    /// </example>
    public NotMyCodeAttribute(LicenseIdentifier license)
    {
        License = license;
        Permission = "This code is used legally as it is licensed under the " + license.Name + " license.";
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
    /// <remarks>
    /// If you want to get the license identifier from a string, do not use this constructor.
    /// Instead, use the <see cref="NotMyCodeAttribute(LicenseIdentifier)"/> constructor and use the method <see cref="LicenseIdentifier.TryParse(string, out LicenseIdentifier)"/>.
    /// </remarks>
    /// <param name="permissionMessage">The written permission the author of this software gave you to use the code associated with this attribute.</param>
    /// <example>
    /// [NotMyCode(".NET (including the runtime repo) is licensed under the MIT license.")]
    /// public struct HashCode
    /// </example>
    public NotMyCodeAttribute(string permissionMessage)
    {
        License = null;
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
