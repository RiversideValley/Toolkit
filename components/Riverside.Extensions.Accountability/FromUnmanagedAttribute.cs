namespace Riverside.Extensions.Accountability;

/// <summary>
/// Attribute used to indicate that a method is implemented in unmanaged code.
/// </summary>
/// <remarks>
/// This attribute does not have any effect on the code itself, and is used for metadata and accountability.
/// </remarks>
[AttributeUsage(AttributeTargets.All)]
public sealed class FromUnmanagedAttribute : Attribute
{
    /// <summary>
    /// Indicates that the method is pulled from or implemented in unmanaged code.
    /// </summary>
    public FromUnmanagedAttribute()
    {
    }

    /// <summary>
    /// Indicates that the method is pulled from or implemented in unmanaged code.
    /// </summary>
    /// <example>
    /// [FromUnmanaged("ntshrui.dll", "CSharingConfiguration::ShowShareItemsUI(HWND *, IShellItemArray *)")]
    /// HRESULT ShowShareItemsUI(HWND hwnd, IShellItemArray items);
    /// </example>
    /// <param name="dll">The library that the method is implemented in</param>
    /// <param name="method">The method signature of the function, e.g. <c>CSharingConfiguration::ShowShareItemsUI(HWND *, IShellItemArray *)</c></param>
    public FromUnmanagedAttribute(string dll, string method)
    {
        Library = dll;
        Function = method;
        EntryPoint = GetEntryPoint(dll, method);
    }

    /// <summary>
    /// Indicates that the method is pulled from or implemented in unmanaged code.
    /// </summary>
    /// <example>
    /// [FromUnmanaged("CSharingConfiguration::ShowShareItemsUI(HWND *, IShellItemArray *)")]
    /// HRESULT ShowShareItemsUI(HWND hwnd, IShellItemArray items);
    /// </example>
    /// <param name="method">The method signature of the function, e.g. <c>CSharingConfiguration::ShowShareItemsUI(HWND *, IShellItemArray *)</c></param>
    public FromUnmanagedAttribute(string method)
    {
        Function = method;
        EntryPoint = method;
    }

    /// <summary>
    /// The library that the method is implemented in.
    /// </summary>
    /// <example>
    /// <c>ntshrui.dll</c>
    /// </example>
    public string Library { get; }

    /// <summary>
    /// The library that the method is implemented in, without the file extension.
    /// </summary>
    /// <example>
    /// <c>ntshrui</c>
    /// </example>
    public string LibraryWithoutExtension => GetLibraryWithoutExtension(Library);

    /// <summary>
    /// The method signature of the function.
    /// </summary>
    /// <example>
    /// <c>CSharingConfiguration::ShowShareItemsUI(HWND *, IShellItemArray *)</c>
    /// </example>
    public string Function { get; }

    /// <summary>
    /// The entry point of the function.
    /// </summary>
    /// <example>
    /// <c>ntshrui!CSharingConfiguration::ShowShareItemsUI(HWND *, IShellItemArray *)</c>
    /// </example>
    public string EntryPoint { get; }

    private static string GetEntryPoint(string dll, string method)
    {
        string libraryWithoutExtension = GetLibraryWithoutExtension(dll);
        return $"{libraryWithoutExtension}!{method}";
    }

    private static string GetLibraryWithoutExtension(string dll)
    {
        return dll
            .Replace(".dll", "")
            .Replace(".so", "")
            .Replace(".dylib", "");
    }
}
