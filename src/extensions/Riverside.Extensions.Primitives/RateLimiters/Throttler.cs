namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides a throttling mechanism to limit the rate at which an action is executed.
    /// </summary>
    /// <param name="maxRequests">The maximum number of requests allowed in the specified time window.</param>
    /// <param name="timeWindow">The time window for the rate limit.</param>
    public class Throttler(int maxRequests, TimeSpan timeWindow)
    {
        private readonly SemaphoreSlim _semaphore = new(maxRequests, maxRequests);
        private readonly TimeSpan _timeWindow = timeWindow;
        private DateTime _lastExecutionTime = DateTime.MinValue;

        /// <summary>
        /// Executes the specified action with throttling.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ThrottleAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                DateTime now = DateTime.UtcNow;
                TimeSpan timeSinceLastExecution = now - _lastExecutionTime;

                if (timeSinceLastExecution < _timeWindow)
                {
                    await Task.Delay(_timeWindow - timeSinceLastExecution, cancellationToken);
                }

                _lastExecutionTime = DateTime.UtcNow;
                await action();
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
