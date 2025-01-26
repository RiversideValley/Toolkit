namespace Riverside.Extensions.Circuits
{
    /// <summary>
    /// Represents a circuit breaker that can be used to protect resources from being overwhelmed.
    /// </summary>
    /// <param name="maxFailures">The maximum number of failures before the circuit breaker opens.</param>
    /// <param name="resetTimeout">The time to wait before attempting to reset the circuit breaker.</param>
    public class CircuitBreaker(int maxFailures, TimeSpan resetTimeout)
    {
        private readonly int _maxFailures = maxFailures;
        private readonly TimeSpan _resetTimeout = resetTimeout;
        private int _failureCount = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;
        private bool _isOpen = false;

        /// <summary>
        /// Executes the specified operation with circuit breaker protection.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the circuit breaker is open.</exception>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            if (_isOpen)
            {
                if (DateTime.UtcNow - _lastFailureTime > _resetTimeout)
                {
                    // Reset the circuit breaker
                    _isOpen = false;
                    _failureCount = 0;
                }
                else
                {
                    throw new InvalidOperationException("Circuit breaker is open.");
                }
            }

            try
            {
                T result = await operation();
                _failureCount = 0; // Reset failure count on success
                return result;
            }
            catch
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                if (_failureCount >= _maxFailures)
                {
                    _isOpen = true;
                }

                throw;
            }
        }
    }
}
