using System;
using System.Threading.Tasks;

namespace Riverside.Extensions.Policies
{
    /// <summary>
    /// Provides a retry policy to handle transient failures.
    /// </summary>
    public static class RetryPolicy
    {
        /// <summary>
        /// Executes the specified operation with a retry policy.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="maxRetries">The maximum number of retries. Must be greater than or equal to 1.</param>
        /// <param name="delay">The delay between retries. Defaults to 2 seconds if not specified.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when maxRetries is less than 1.</exception>
        public static async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, int maxRetries = 3, TimeSpan? delay = null)
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Max retries must be greater than or equal to 1.");
            }

            delay ??= TimeSpan.FromSeconds(2);

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"Attempt {attempt} failed: {ex.Message}. Retrying in {delay.Value.TotalSeconds} seconds...");
                    await Task.Delay(delay.Value);
                }
            }

            // If the operation still fails after the maximum number of retries, rethrow the last exception
            return await operation();
        }
    }
}
