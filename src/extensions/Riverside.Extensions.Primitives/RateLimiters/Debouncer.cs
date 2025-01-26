namespace Riverside.Extensions.RateLimiters;

/// <summary>
/// Provides a debouncing mechanism to limit the rate at which an action is executed.
/// </summary>
public class Debouncer(TimeSpan delay)
{
    private readonly TimeSpan _delay = delay;
    private CancellationTokenSource _cancellationTokenSource = new();

    /// <summary>
    /// Executes the specified action after a delay, canceling any previously scheduled actions.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DebounceAsync(Func<Task> action)
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(_delay, _cancellationTokenSource.Token);
            await action();
        }
        catch (TaskCanceledException)
        {
            // Task was canceled, do nothing
        }
    }
}
