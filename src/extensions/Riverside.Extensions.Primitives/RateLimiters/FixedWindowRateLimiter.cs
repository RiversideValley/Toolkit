using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides a fixed window rate limiter to control the rate of operations.
    /// </summary>
    public class FixedWindowRateLimiter(int maxRequests, TimeSpan timeWindow)
    {
        private readonly int _maxRequests = maxRequests;
        private readonly TimeSpan _timeWindow = timeWindow;
        private readonly ConcurrentDictionary<DateTime, int> _requestCounts = new();
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

                // Remove old entries
                foreach (DateTime key in _requestCounts.Keys)
                {
                    if (key < windowStart)
                    {
                        _ = _requestCounts.TryRemove(key, out _);
                    }
                }

                // Calculate the total number of requests in the current window
                int requestCount = 0;
                foreach (int count in _requestCounts.Values)
                {
                    requestCount += count;
                }

                if (requestCount < _maxRequests)
                {
                    _ = _requestCounts.AddOrUpdate(now, 1, (key, value) => value + 1);
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
                await Task.Delay(_timeWindow, cancellationToken);
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
                await Task.Delay(_timeWindow, cancellationToken);
            }

            await operation();
        }
    }
}
