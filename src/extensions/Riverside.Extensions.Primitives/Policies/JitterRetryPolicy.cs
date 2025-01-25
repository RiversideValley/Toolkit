using System;
using System.Threading;
using System.Threading.Tasks;

namespace Riverside.Extensions.Policies
{
    /// <summary>
    /// Provides a retry policy with jitter to handle transient failures.
    /// </summary>
    public static class JitterRetryPolicy
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Executes the specified operation with jitter between retries.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="maxRetries">The maximum number of retries. Must be greater than or equal to 1.</param>
        /// <param name="initialDelay">The initial delay before the first retry. Defaults to 1 second if not specified.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when maxRetries is less than 1.</exception>
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, int maxRetries = 5, TimeSpan? initialDelay = null, CancellationToken cancellationToken = default)
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than or equal to 1.");
            }

            initialDelay ??= TimeSpan.FromSeconds(1);
            TimeSpan delay = initialDelay.Value;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delay.TotalSeconds} seconds...");
                    await Task.Delay(AddJitter(delay), cancellationToken);
                    delay = TimeSpan.FromTicks(delay.Ticks * 2); // Exponential backoff
                }
            }

            // If the operation still fails after the maximum number of retries, rethrow the last exception
            return await operation();
        }

        /// <summary>
        /// Adds a randomized delay (jitter) to the specified delay.
        /// </summary>
        /// <param name="delay">The original delay.</param>
        /// <returns>The delay with added jitter.</returns>
        private static TimeSpan AddJitter(TimeSpan delay)
        {
            TimeSpan jitter = TimeSpan.FromMilliseconds(_random.Next(0, (int)delay.TotalMilliseconds));
            return delay + jitter;
        }
    }
}
