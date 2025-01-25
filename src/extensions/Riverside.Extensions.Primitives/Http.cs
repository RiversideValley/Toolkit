using Riverside.Extensions.Circuits;
using Riverside.Extensions.Policies;
using Riverside.Extensions.RateLimiters;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Riverside.Extensions
{
    /// <summary>
    /// Provides various methods to fetch data with different rate limiting and retry policies.
    /// </summary>
    /// <remarks>
    /// This class serves as an example for the various functions Riverside.Extensions exposes.
    /// </remarks>
    public class Http
    {
        private static readonly HttpClient _httpClient = new();
        private static readonly CircuitBreaker _circuitBreaker = new(3, TimeSpan.FromSeconds(30));
        private static readonly RateLimiter _rateLimiter = new(5, TimeSpan.FromSeconds(10)); // 5 requests per 10 seconds
        private static readonly Func<string, Task<string>> _fetchDataMemoized = Memoization.MemoizeAsync<string, string>(FetchDataAsyncWrapper);
        private static readonly Debouncer _debouncer = new(TimeSpan.FromSeconds(1)); // 1 second debounce
        private static readonly Throttler _throttler = new(1, TimeSpan.FromSeconds(2)); // 1 request per 2 seconds
        private static readonly Bulkhead _bulkhead = new(3); // Max 3 concurrent requests
        private static readonly SimpleCache<string, string> _cache = new(TimeSpan.FromMinutes(5)); // 5 minutes cache
        private static readonly SlidingWindowRateLimiter _slidingWindowRateLimiter = new(5, TimeSpan.FromSeconds(10)); // 5 requests per 10 seconds
        private static readonly SemaphoreLock _semaphoreLock = new(3); // Max 3 concurrent requests
        private static readonly AdvancedCircuitBreaker _advancedCircuitBreaker = new(3, TimeSpan.FromSeconds(30));
        private static readonly TokenBucketRateLimiter _tokenBucketRateLimiter = new(10, 1, TimeSpan.FromSeconds(1)); // 10 tokens, 1 token per second
        private static readonly PriorityQueue<(string url, int priority)> _priorityQueue = new();
        private static readonly LeakyBucketRateLimiter _leakyBucketRateLimiter = new(10, TimeSpan.FromSeconds(1)); // 10 requests, 1 request per second
        private static readonly PriorityTaskScheduler _priorityTaskScheduler = new(3); // Max 3 concurrent tasks
        private static readonly FixedWindowRateLimiter _fixedWindowRateLimiter = new(5, TimeSpan.FromSeconds(10)); // 5 requests per 10 seconds
        private static readonly WeightedRoundRobinScheduler<string> _weightedRoundRobinScheduler = new(new List<(string, int)>
        {
            ("https://example.com", 5),
            ("https://example.org", 3),
            ("https://example.net", 2)
        });
        private static readonly SlidingLogRateLimiter _slidingLogRateLimiter = new(5, TimeSpan.FromSeconds(10)); // 5 requests per 10 seconds
        private static readonly DynamicTokenBucketRateLimiter _dynamicTokenBucketRateLimiter = new(10, 1, TimeSpan.FromSeconds(1)); // 10 tokens, 1 token per second
        private static readonly ExponentialBackoffRateLimiter _exponentialBackoffRateLimiter = new(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30)); // 5 retries, initial delay 1 second, max delay 30 seconds

        /// <summary>
        /// Fetches data from the specified URL.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataAsync(string url, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            _ = response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static Task<string> FetchDataAsyncWrapper(string url)
        {
            return FetchDataAsync(url);
        }

        /// <summary>
        /// Fetches data with a retry policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithRetryAsync(string url, CancellationToken cancellationToken = default)
        {
            return await RetryPolicy.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
        }

        /// <summary>
        /// Fetches data with a circuit breaker policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithCircuitBreakerAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _circuitBreaker.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
        }

        /// <summary>
        /// Fetches data with a rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _rateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data with memoization.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataMemoizedAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _fetchDataMemoized(url);
        }

        /// <summary>
        /// Fetches data with a debounce policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task FetchDataDebouncedAsync(string url)
        {
            await _debouncer.DebounceAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                _ = response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            });
        }

        /// <summary>
        /// Fetches data with a throttle policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task FetchDataThrottledAsync(string url, CancellationToken cancellationToken = default)
        {
            await _throttler.ThrottleAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data with a timeout policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="timeout">The timeout duration.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithTimeoutAsync(string url, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            return await TimeoutPolicy.ExecuteAsync(async ct =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, ct);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, timeout);
        }

        /// <summary>
        /// Fetches data with a bulkhead policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithBulkheadAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _bulkhead.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data with caching.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithCacheAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _cache.GetOrAddAsync(url, async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
        }

        /// <summary>
        /// Fetches data with exponential backoff retry policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithExponentialBackoffAsync(string url, CancellationToken cancellationToken = default)
        {
            return await ExponentialBackoffRetryPolicy.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, maxRetries: 5, initialDelay: TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Fetches data with a sliding window rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithSlidingWindowRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _slidingWindowRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data with a semaphore lock.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithSemaphoreLockAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _semaphoreLock.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data with an advanced circuit breaker policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithAdvancedCircuitBreakerAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _advancedCircuitBreaker.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            });
        }

        /// <summary>
        /// Fetches data with a token bucket rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithTokenBucketRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _tokenBucketRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Enqueues a request with a specified priority.
        /// </summary>
        /// <param name="url">The URL of the request.</param>
        /// <param name="priority">The priority of the request.</param>
        public static void EnqueueRequest(string url, int priority)
        {
            _priorityQueue.Enqueue((url, priority), priority);
        }

        /// <summary>
        /// Processes the next request in the priority queue.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> ProcessNextRequestAsync(CancellationToken cancellationToken = default)
        {
            (string url, _) = _priorityQueue.Dequeue();
            return await FetchDataAsync(url, cancellationToken);
        }

        /// <summary>
        /// Fetches data with a leaky bucket rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithLeakyBucketRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _leakyBucketRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Enqueues a task with a specified priority.
        /// </summary>
        /// <param name="task">The task to enqueue.</param>
        /// <param name="priority">The priority of the task.</param>
        public static void EnqueueTask(Func<Task> task, int priority)
        {
            _priorityTaskScheduler.EnqueueTask(task, priority);
        }

        /// <summary>
        /// Fetches data with a fixed window rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithFixedWindowRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _fixedWindowRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data using a weighted round-robin scheduler.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithWeightedRoundRobinAsync(CancellationToken cancellationToken = default)
        {
            string serverUrl = _weightedRoundRobinScheduler.GetNext();
            return await FetchDataAsync(serverUrl, cancellationToken);
        }

        /// <summary>
        /// Fetches data with a jitter retry policy.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithJitterRetryAsync(string url, CancellationToken cancellationToken = default)
        {
            return await JitterRetryPolicy.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, maxRetries: 5, initialDelay: TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Fetches data with a sliding log rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithSlidingLogRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _slidingLogRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Fetches data with a dynamic token bucket rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithDynamicTokenBucketRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _dynamicTokenBucketRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// Updates the rate limit of the dynamic token bucket rate limiter.
        /// </summary>
        public static void UpdateRateLimit(int newBucketCapacity, int newTokensPerInterval)
        {
            _dynamicTokenBucketRateLimiter.UpdateRateLimit(newBucketCapacity, newTokensPerInterval);
        }

        /// <summary>
        /// Fetches data with an exponential backoff rate limiter.
        /// </summary>
        /// <param name="url">The URL to fetch data from.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The fetched data as a string.</returns>
        public static async Task<string> FetchDataWithExponentialBackoffRateLimiterAsync(string url, CancellationToken cancellationToken = default)
        {
            return await _exponentialBackoffRateLimiter.ExecuteAsync(async () =>
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }, cancellationToken);
        }

    }
}
