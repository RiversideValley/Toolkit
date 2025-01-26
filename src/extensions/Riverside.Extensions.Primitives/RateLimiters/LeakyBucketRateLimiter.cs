namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides a leaky bucket rate limiter to control the rate of operations.
    /// </summary>
    public class LeakyBucketRateLimiter(int bucketCapacity, TimeSpan leakInterval)
    {
        private readonly int _bucketCapacity = bucketCapacity;
        private readonly TimeSpan _leakInterval = leakInterval;
        private int _currentBucketSize = 0;
        private DateTime _lastLeakTime = DateTime.UtcNow;
        private readonly object _lock = new();

        /// <summary>
        /// Attempts to consume a token from the bucket.
        /// </summary>
        /// <returns>True if a token was consumed; otherwise, false.</returns>
        public bool TryConsume()
        {
            lock (_lock)
            {
                Leak();

                if (_currentBucketSize < _bucketCapacity)
                {
                    _currentBucketSize++;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Leaks tokens from the bucket based on the elapsed time.
        /// </summary>
        private void Leak()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan timeSinceLastLeak = now - _lastLeakTime;

            if (timeSinceLastLeak > _leakInterval)
            {
                int leaks = (int)(timeSinceLastLeak.TotalMilliseconds / _leakInterval.TotalMilliseconds);
                _currentBucketSize = Math.Max(0, _currentBucketSize - leaks);
                _lastLeakTime = now;
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
                await Task.Delay(_leakInterval, cancellationToken);
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
                await Task.Delay(_leakInterval, cancellationToken);
            }

            await operation();
        }
    }
}
