using System.Collections.Concurrent;

namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides a sliding window rate limiter to control the rate of operations.
    /// </summary>
    public class SlidingWindowRateLimiter(int maxRequests, TimeSpan timeWindow)
    {
        private readonly int _maxRequests = maxRequests;
        private readonly TimeSpan _timeWindow = timeWindow;
        private readonly ConcurrentQueue<DateTime> _requestTimestamps = new();

        /// <summary>
        /// Determines whether a request is allowed based on the rate limit.
        /// </summary>
        /// <returns>True if the request is allowed; otherwise, false.</returns>
        public bool IsRequestAllowed()
        {
            DateTime now = DateTime.UtcNow;

            // Remove timestamps outside the time window
            while (_requestTimestamps.TryPeek(out DateTime timestamp) && now - timestamp > _timeWindow)
            {
                _ = _requestTimestamps.TryDequeue(out _);
            }

            if (_requestTimestamps.Count < _maxRequests)
            {
                _requestTimestamps.Enqueue(now);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the specified operation, ensuring that the rate of operations does not exceed the specified limit.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            while (!IsRequestAllowed())
            {
                await Task.Delay((int)(_timeWindow.TotalMilliseconds / _maxRequests), cancellationToken);
            }

            return await operation();
        }

        /// <summary>
        /// Executes the specified operation, ensuring that the rate of operations does not exceed the specified limit.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            while (!IsRequestAllowed())
            {
                await Task.Delay((int)(_timeWindow.TotalMilliseconds / _maxRequests), cancellationToken);
            }

            await operation();
        }
    }
}
