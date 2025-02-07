using Windows.ApplicationModel;

namespace Riverside.Toolkit.Helpers;

/// <summary>
/// Provides helper methods for design-time functionality.
/// </summary>
/// <remarks>
/// This helper is introduced in CubeKit to backport the Windows Community Toolkit version of the DesignTimeHelpers class that is not compatible with all target frameworks.
/// </remarks>
[Obsolete("Riverside.Toolkit.Helpers.DesignTimeHelpers is obsolete, use CommunityToolkit.WinUI.Helpers.DesignTimeHelpers instead.", UrlFormat = "https://github.com/CommunityToolkit/Windows/blob/main/components/Helpers/src/DesignTimeHelpers.cs")]
public static class DesignTimeHelpers
{
    private static readonly Lazy<bool> designModeEnabled = new(InitializeDesignerMode);

    private static readonly Lazy<bool> designMode2Enabled = new(InitializeDesignMode2);

    /// <summary>
    /// Gets a value indicating whether app is running in the Legacy Designer
    /// </summary>
    public static bool IsRunningInLegacyDesignerMode => designModeEnabled.Value && !designMode2Enabled.Value;

    /// <summary>
    /// Gets a value indicating whether app is running in the Enhanced Designer
    /// </summary>
    public static bool IsRunningInEnhancedDesignerMode => designModeEnabled.Value && designMode2Enabled.Value;

    /// <summary>
    /// Gets a value indicating whether app is not running in the Designer
    /// </summary>
    public static bool IsRunningInApplicationRuntimeMode => !designModeEnabled.Value;

    // Private initializer
    private static bool InitializeDesignerMode() => DesignMode.DesignModeEnabled;

    /// <summary>
    /// Used to enable or disable user code inside a XAML designer that targets the Windows 10 Fall Creators Update SDK, or later.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="DesignMode.DesignModeEnabled"/> returns true when called from user code running inside any version of the XAML designer--regardless of which SDK version you target. This check is recommended for most users.
    /// </para><para>
    /// Starting with the Windows 10 Fall Creators Update, Visual Studio provides a new XAML designer that targets the Windows 10 Fall Creators Update and later.
    /// </para><para>
    /// <see cref="DesignMode.DesignMode2Enabled"/> to differentiate code that depends on functionality only enabled for a XAML designer that targets the Windows 10 Fall Creators Update SDK or later.
    /// </para>
    /// <para>
    /// More info here: https://docs.microsoft.com/en-us/uwp/api/Windows.ApplicationModel.DesignMode
    /// </para>
    /// </remarks>
    /// <returns>True if called from code running inside a XAML designer that targets the Windows 10 Fall Creators Update, or later; otherwise false.</returns>
    private static bool InitializeDesignMode2() => DesignMode.DesignMode2Enabled;
}
