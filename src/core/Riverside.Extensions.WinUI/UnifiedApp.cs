// This file contains portions of code from the Files App project, licensed under the MIT License.

using System.Reflection;

namespace Riverside.Extensions.WinUI;

/// <summary>
/// Represents a unified application class that can be used across all platforms.
/// It provides a common set of properties and methods that can be used in any project.
/// </summary>
/// <seealso cref="Application"/>
public abstract partial class UnifiedApp : Application
{
    private static CommandBarFlyout? _LastOpenedFlyout;

#if !Wpf && !Uwp
    /// <summary>
    /// Gets a dictionary of windows identified by a string key.
    /// </summary>
    public Dictionary<string, Window> Windows { get; } = [];
#endif

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public static readonly string AppVersion =
#if Wpf
        $"{Assembly.GetEntryAssembly()?.GetName().Version?.Major}." +
        $"{Assembly.GetEntryAssembly()?.GetName().Version?.Minor}." +
        $"{Assembly.GetEntryAssembly()?.GetName().Version?.Build}." +
        $"{Assembly.GetEntryAssembly()?.GetName().Version?.Revision}";
#else
        $"{Package.Current.Id.Version.Major}." +
        $"{Package.Current.Id.Version.Minor}." +
        $"{Package.Current.Id.Version.Build}." +
        $"{Package.Current.Id.Version.Revision}";
#endif

    /// <summary>
    /// Gets the current <see cref="UnifiedApp"/> instance in use.
    /// </summary>
    public new static UnifiedApp Current
        => (UnifiedApp)Application.Current;

    /// <summary>
    /// Gets or sets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; set; }

#if !Wpf
    /// <summary>
    /// Gets the application data container for local settings.
    /// </summary>
    public ApplicationDataContainer Settings = ApplicationData.Current.LocalSettings;
#endif

#if !Wpf && !Uwp
    /// <summary>
    /// Opens a new window with the specified key, page type, and optional parameter.
    /// </summary>
    /// <typeparam name="TPage">The type of the page to navigate to.</typeparam>
    /// <param name="windowKey">The key to identify the window.</param>
    /// <param name="parameter">The parameter to pass to the page.</param>
    public void OpenNewWindow<TPage>(string windowKey, object parameter = null)
    {
        if (Windows.TryGetValue(windowKey, out Window value))
        {
            value.Activate();
            return;
        }

        Window newWindow = new();
        Frame frame = new();
        _ = frame.Navigate(typeof(TPage), parameter);
        newWindow.Content = frame;
        newWindow.Activate();

        Windows[windowKey] = newWindow;
    }
#endif

    /// <summary>
    /// Gets or sets the last opened <see cref="CommandBarFlyout"/>.
    /// </summary>
    public static CommandBarFlyout? LastOpenedFlyout
    {
        set
        {
            _LastOpenedFlyout = value;

            if (_LastOpenedFlyout is not null)
                _LastOpenedFlyout.Closed += LastOpenedFlyout_Closed;
        }
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The <see cref="Frame"/> which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    public virtual void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        => throw new Exception("Failed to load Page "
#if Wpf
            + e.Uri);
#else
            + e.SourcePageType.FullName);
#endif

#if !Wpf
    /// <summary>
    /// Invoked when the application execution is being suspended.
    /// </summary>
    /// <remarks>
    /// This is the standard method to save application state before the application is suspended.
    /// You can override it to save additional state, but you should always call the base implementation.
    /// </remarks>
    /// <param name="sender">The source of the suspend request.</param>
    /// <param name="e">Details about the suspend request.</param>
    public virtual void OnSuspending(object sender, SuspendingEventArgs e)
    {
        SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
        deferral.Complete();
    }
#endif

    /// <summary>
    /// Invoked when an unobserved task exception occurs.
    /// </summary>
    /// <param name="sender">The source of the unobserved task exception.</param>
    /// <param name="e">Details about the unobserved task exception.</param>
    public virtual void OnUnobservedException(object? sender, UnobservedTaskExceptionEventArgs e)
        => e.SetObserved();

    /// <summary>
    /// Invoked when an unhandled exception occurs.
    /// </summary>
    /// <remarks>
    /// This is the standard method to handle unhandled exceptions.
    /// You can override it to provide custom exception handling, but you should always call the base implementation.
    /// 
    /// This base version also raises a new <see cref="ContentDialog"/> to display the exception message.
    /// </remarks>
    /// <param name="sender">The source of the unhandled exception.</param>
    /// <param name="e">Details about the unhandled exception.</param>
    public async virtual void OnUnhandledException(object? sender,
#if Uwp
        Windows.UI.Xaml.UnhandledExceptionEventArgs e)
#elif WinUI
        Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
#else
        UnhandledExceptionEventArgs e)
#endif
    {
        try
        {
            await new ContentDialog
            {
                Title = "Unhandled exception",
                Content =
#if Uwp
                e.Message,
#else
                "An unhandled exception occured.",
#endif
                CloseButtonText = "Close"
            }
                .ShowAsync();
        }
        catch { }
    }

    /// <summary>
    /// Gets invoked when the last opened flyout is closed.
    /// </summary>
    private static void LastOpenedFlyout_Closed(object? sender, object e)
    {
        if (sender is not CommandBarFlyout commandBarFlyout)
            return;

        commandBarFlyout.Closed -= LastOpenedFlyout_Closed;
        if (_LastOpenedFlyout == commandBarFlyout)
            _LastOpenedFlyout = null;
    }
}