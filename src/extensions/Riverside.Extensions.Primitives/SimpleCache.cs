using System.Collections.Concurrent;

namespace Riverside.Extensions
{
    /// <summary>
    /// Provides a simple cache with expiration functionality.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the cache.</typeparam>
    /// <typeparam name="TValue">The type of the values in the cache.</typeparam>
    public class SimpleCache<TKey, TValue>(TimeSpan defaultExpiration)
    {
        private readonly ConcurrentDictionary<TKey, CacheItem> _cache = new();
        private readonly TimeSpan _defaultExpiration = defaultExpiration;

        /// <summary>
        /// Sets a value in the cache with an optional expiration time.
        /// </summary>
        /// <param name="key">The key of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="expiration">The expiration time for the cache item. If null, the default expiration time is used.</param>
        public void Set(TKey key, TValue value, TimeSpan? expiration = null)
        {
            CacheItem cacheItem = new(value, DateTime.UtcNow + (expiration ?? _defaultExpiration));
            _cache[key] = cacheItem;
        }

        /// <summary>
        /// Tries to get a value from the cache.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>True if the cache contains an element with the specified key and it has not expired; otherwise, false.</returns>
        public bool TryGet(TKey key, out TValue value)
        {
            if (_cache.TryGetValue(key, out CacheItem cacheItem) && cacheItem.Expiration > DateTime.UtcNow)
            {
                value = cacheItem.Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets a value from the cache or adds it if it does not exist.
        /// </summary>
        /// <param name="key">The key of the value to get or add.</param>
        /// <param name="valueFactory">The function to generate the value if it does not exist in the cache.</param>
        /// <param name="expiration">The expiration time for the cache item. If null, the default expiration time is used.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the value associated with the specified key.</returns>
        public async Task<TValue> GetOrAddAsync(TKey key, Func<Task<TValue>> valueFactory, TimeSpan? expiration = null)
        {
            if (TryGet(key, out TValue value))
            {
                return value;
            }

            value = await valueFactory();
            Set(key, value, expiration);
            return value;
        }

        private class CacheItem(TValue value, DateTime expiration)
        {

            /// <summary>
            /// Gets the value of the cache item.
            /// </summary>
            public TValue Value { get; } = value;

            /// <summary>
            /// Gets the expiration time of the cache item.
            /// </summary>
            public DateTime Expiration { get; } = expiration;
        }
    }
}
