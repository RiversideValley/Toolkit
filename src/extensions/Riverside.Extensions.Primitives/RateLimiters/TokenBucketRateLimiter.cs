namespace Riverside.Extensions.RateLimiters
{
    /// <summary>
    /// Provides a token bucket rate limiter to control the rate of operations.
    /// </summary>
    public class TokenBucketRateLimiter(int bucketCapacity, int tokensPerInterval, TimeSpan interval)
    {
        private readonly int _bucketCapacity = bucketCapacity;
        private readonly int _tokensPerInterval = tokensPerInterval;
        private readonly TimeSpan _interval = interval;
        private int _tokens = bucketCapacity;
        private DateTime _lastRefill = DateTime.UtcNow;
        private readonly object _lock = new();

        /// <summary>
        /// Attempts to consume a token from the bucket.
        /// </summary>
        /// <returns>True if a token was consumed; otherwise, false.</returns>
        public bool TryConsume()
        {
            lock (_lock)
            {
                RefillTokens();

                if (_tokens > 0)
                {
                    _tokens--;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Refills the tokens in the bucket based on the elapsed time.
        /// </summary>
        private void RefillTokens()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan timeSinceLastRefill = now - _lastRefill;

            if (timeSinceLastRefill > _interval)
            {
                int tokensToAdd = (int)(timeSinceLastRefill.TotalMilliseconds / _interval.TotalMilliseconds) * _tokensPerInterval;
                _tokens = Math.Min(_bucketCapacity, _tokens + tokensToAdd);
                _lastRefill = now;
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
                await Task.Delay(_interval, cancellationToken);
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
                await Task.Delay(_interval, cancellationToken);
            }

            await operation();
        }
    }
}
