using System;
using System.Threading;
using System.Threading.Tasks;

namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides an exponential backoff rate limiter to control the rate of operations.
    /// </summary>
    public class ExponentialBackoffRateLimiter(int maxRetries, TimeSpan initialDelay, TimeSpan maxDelay)
    {
        private readonly int _maxRetries = maxRetries;
        private readonly TimeSpan _initialDelay = initialDelay;
        private readonly TimeSpan _maxDelay = maxDelay;
        private readonly object _lock = new();

        /// <summary>
        /// Executes the specified operation with exponential backoff between retries.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            int attempt = 0;
            TimeSpan delay = _initialDelay;

            while (attempt < _maxRetries)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (attempt < _maxRetries)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delay.TotalSeconds} seconds...");
                    await Task.Delay(delay, cancellationToken);
                    delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, _maxDelay.Ticks)); // Exponential backoff
                }
            }

            // If the operation still fails after the maximum number of retries, rethrow the last exception
            return await operation();
        }

        /// <summary>
        /// Executes the specified operation with exponential backoff between retries.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            int attempt = 0;
            TimeSpan delay = _initialDelay;

            while (attempt < _maxRetries)
            {
                try
                {
                    await operation();
                    return;
                }
                catch (Exception ex) when (attempt < _maxRetries)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delay.TotalSeconds} seconds...");
                    await Task.Delay(delay, cancellationToken);
                    delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, _maxDelay.Ticks)); // Exponential backoff
                }
            }

            // If the operation still fails after the maximum number of retries, rethrow the last exception
            await operation();
        }
    }
}
