namespace Riverside.Extensions.WinUI;

public class UnifiedApp : Application
{
    /// <summary>
    /// Gets the current <see cref="UnifiedApp"/> instance in use
    /// </summary>
    public new static UnifiedApp Current
        => (UnifiedApp)Application.Current;

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    public virtual void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        => throw new Exception("Failed to load Page "
#if Wpf
            + e.Uri);
#else
            + e.SourcePageType.FullName);
#endif

    public virtual void OnUnobservedException(object? sender, UnobservedTaskExceptionEventArgs e)
        => e.SetObserved();

    public async virtual void OnUnhandledException(object? sender,
#if UWP
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
#if UWP
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
}