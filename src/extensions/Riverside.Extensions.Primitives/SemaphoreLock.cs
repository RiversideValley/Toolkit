namespace Riverside.Extensions
{
    /// <summary>
    /// Provides a semaphore lock to limit the number of concurrent requests.
    /// </summary>
    public class SemaphoreLock(int maxConcurrentRequests)
    {
        private readonly SemaphoreSlim _semaphore = new(maxConcurrentRequests, maxConcurrentRequests);

        /// <summary>
        /// Executes the specified operation with semaphore lock protection.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                return await operation();
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }

        /// <summary>
        /// Executes the specified operation with semaphore lock protection.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                await operation();
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
