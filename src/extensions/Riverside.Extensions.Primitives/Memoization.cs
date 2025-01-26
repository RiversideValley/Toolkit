using System.Collections.Concurrent;

namespace Riverside.Extensions
{
    /// <summary>
    /// Provides methods for memoizing functions to cache their results.
    /// </summary>
    public static class Memoization
    {
        /// <summary>
        /// Memoizes the specified function to cache its results.
        /// </summary>
        /// <typeparam name="T">The type of the input parameter of the function.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
        /// <param name="func">The function to memoize.</param>
        /// <returns>A memoized version of the specified function.</returns>
        public static Func<T, TResult> Memoize<T, TResult>(Func<T, TResult> func)
        {
            ConcurrentDictionary<T, TResult> cache = new();
            return arg =>
            {
                if (cache.TryGetValue(arg, out TResult result))
                {
                    return result;
                }
                result = func(arg);
                cache[arg] = result;
                return result;
            };
        }

        /// <summary>
        /// Memoizes the specified asynchronous function to cache its results.
        /// </summary>
        /// <typeparam name="T">The type of the input parameter of the function.</typeparam>
        /// <typeparam name="TResult">The type of the result produced by the function.</typeparam>
        /// <param name="func">The asynchronous function to memoize.</param>
        /// <returns>A memoized version of the specified asynchronous function.</returns>
        public static Func<T, Task<TResult>> MemoizeAsync<T, TResult>(Func<T, Task<TResult>> func)
        {
            ConcurrentDictionary<T, Task<TResult>> cache = new();
            return async arg =>
            {
                if (cache.TryGetValue(arg, out Task<TResult> result))
                {
                    return await result;
                }
                result = func(arg);
                cache[arg] = result;
                return await result;
            };
        }
    }
}
