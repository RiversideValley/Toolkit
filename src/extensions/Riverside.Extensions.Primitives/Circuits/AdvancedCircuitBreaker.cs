using System;
using System.Threading.Tasks;

namespace Riverside.Extensions.Circuits
{
    /// <summary>
    /// Represents an advanced circuit breaker that can be used to protect resources from being overwhelmed.
    /// </summary>
    public class AdvancedCircuitBreaker(int maxFailures, TimeSpan resetTimeout)
    {
        private readonly int _maxFailures = maxFailures;
        private readonly TimeSpan _resetTimeout = resetTimeout;
        private int _failureCount = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;
        private CircuitBreakerState _state = CircuitBreakerState.Closed;
        private readonly object _lock = new();

        /// <summary>
        /// Executes the specified operation with circuit breaker protection.
        /// </summary>
        /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the circuit breaker is open.</exception>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            lock (_lock)
            {
                if (_state == CircuitBreakerState.Open)
                {
                    _state = DateTime.UtcNow - _lastFailureTime > _resetTimeout
                        ? CircuitBreakerState.HalfOpen
                        : throw new InvalidOperationException("Circuit breaker is open.");
                }
            }

            try
            {
                T result = await operation();
                lock (_lock)
                {
                    _failureCount = 0;
                    _state = CircuitBreakerState.Closed;
                }
                return result;
            }
            catch
            {
                lock (_lock)
                {
                    _failureCount++;
                    _lastFailureTime = DateTime.UtcNow;

                    _state = _failureCount >= _maxFailures ? CircuitBreakerState.Open : CircuitBreakerState.HalfOpen;
                }
                throw;
            }
        }

        /// <summary>
        /// Represents the state of the circuit breaker.
        /// </summary>
        private enum CircuitBreakerState
        {
            /// <summary>
            /// The circuit breaker is closed, allowing operations to proceed.
            /// </summary>
            Closed,
            /// <summary>
            /// The circuit breaker is open, blocking operations.
            /// </summary>
            Open,
            /// <summary>
            /// The circuit breaker is half-open, allowing a limited number of operations to proceed.
            /// </summary>
            HalfOpen
        }
    }
}
