using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides a sliding log rate limiter to control the rate of operations.
    /// </summary>
    public class SlidingLogRateLimiter(int maxRequests, TimeSpan timeWindow)
    {
        private readonly int _maxRequests = maxRequests;
        private readonly TimeSpan _timeWindow = timeWindow;
        private readonly ConcurrentQueue<DateTime> _requestLog = new();
        private readonly object _lock = new();

        /// <summary>
        /// Attempts to consume a token from the rate limiter.
        /// </summary>
        /// <returns>True if a token was consumed; otherwise, false.</returns>
        public bool TryConsume()
        {
            lock (_lock)
            {
                DateTime now = DateTime.UtcNow;
                DateTime windowStart = now - _timeWindow;

                // Remove timestamps outside the time window
                while (_requestLog.TryPeek(out DateTime timestamp) && timestamp < windowStart)
                {
                    _ = _requestLog.TryDequeue(out _);
                }

                if (_requestLog.Count < _maxRequests)
                {
                    _requestLog.Enqueue(now);
                    return true;
                }

                return false;
            }
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
            while (!TryConsume())
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
            while (!TryConsume())
            {
                await Task.Delay((int)(_timeWindow.TotalMilliseconds / _maxRequests), cancellationToken);
            }

            await operation();
        }
    }
}
