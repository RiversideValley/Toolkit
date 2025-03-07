using System.Text;

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
    /// <seealso cref="NotMyCodeAttribute(string, string, string?, string?)"/>
    /// <seealso cref="NotMyCodeAttribute(string, string)"/>
    public NotMyCodeAttribute()
    {
        License = string.Empty;
        Permission = "This code is used legally under the terms of the original license.";
        CopyrightYear = DateTime.Now.Year.ToString();
        Copyright = GetCopyrightStatement(
            string.Empty,
            DateTime.Now.Year.ToString());
    }

    /// <summary>
    /// Indicates that the code is not owned by you.
    /// </summary>
    /// <param name="license">The SPDX expression of the license that the code associated with this attribute is licensed under.</param>
    /// <param name="source">The source of the code associated with this attribute.</param>
    /// <param name="holder"></param>
    /// <param name="copyrightYear"></param>
    /// <example>
    /// [NotMyCode("MIT")]
    /// public struct HashCode
    /// </example>
    public NotMyCodeAttribute(string license, string source, string? holder, string? copyrightYear)
    {
        License = license;
        Permission = "This code is used legally as it is licensed under the " + license + " license.";
        CopyrightYear = copyrightYear ?? DateTime.Now.Year.ToString();
        CopyrightHolder = holder;
        Copyright = GetCopyrightStatement(
            license,
            copyrightYear ?? DateTime.Now.Year.ToString(),
            holder: holder);
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
        License = license ?? string.Empty;
        Permission = permissionMessage;
        CopyrightYear = DateTime.Now.Year.ToString();
        Copyright = GetCopyrightStatement(
            license ?? string.Empty,
            DateTime.Now.Year.ToString());
    }

    /// <summary>
    /// The permission message that the author of this software gave you to use the code associated with this attribute.
    /// </summary>
    public string Permission;

    /// <summary>
    /// The license that the code associated with this attribute is licensed under.
    /// </summary>
    public string License;

    public string CopyrightYear;

    public string? CopyrightHolder;

    public string Copyright;

    private static string GetCopyrightStatement(string license, string year, bool implicitAllRightsReserved = true, string? holder = null)
    {
        StringBuilder copyrightText = new();
        copyrightText.Append($"Copyright (c) {year}");

        if (holder is null)
            copyrightText.Append('.');
        else
            copyrightText.Append($" {holder}.");

        if (!implicitAllRightsReserved)
            copyrightText.Append(" All rights reserved.");

        return copyrightText.ToString();
    }
}
