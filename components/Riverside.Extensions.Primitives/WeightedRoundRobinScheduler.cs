namespace Riverside.Extensions;

/// <summary>
/// Provides a weighted round-robin scheduler to distribute load based on weights.
/// </summary>
/// <typeparam name="T">The type of elements to be scheduled.</typeparam>
public class WeightedRoundRobinScheduler<T>(IEnumerable<(T item, int weight)> items)
{
    private readonly List<(T item, int weight)> _items = items.ToList();
    private int _currentIndex = -1;
    private int _currentWeight = 0;
    private readonly object _lock = new();

    /// <summary>
    /// Gets the next item based on the weighted round-robin scheduling algorithm.
    /// </summary>
    /// <returns>The next item.</returns>
    public T GetNext()
    {
        lock (_lock)
        {
            while (true)
            {
                _currentIndex = (_currentIndex + 1) % _items.Count;
                if (_currentIndex == 0)
                {
                    _currentWeight -= GCD();
                    if (_currentWeight <= 0)
                    {
                        _currentWeight = MaxWeight();
                        if (_currentWeight == 0)
                        {
                            return default;
                        }
                    }
                }

                if (_items[_currentIndex].weight >= _currentWeight)
                {
                    return _items[_currentIndex].item;
                }
            }
        }
    }

    /// <summary>
    /// Calculates the greatest common divisor (GCD) of the weights.
    /// </summary>
    /// <returns>The GCD of the weights.</returns>
    private int GCD()
    {
        int gcd = _items[0].weight;
        foreach ((T item, int weight) item in _items)
        {
            gcd = GCD(gcd, item.weight);
        }
        return gcd;
    }

    /// <summary>
    /// Calculates the GCD of two integers.
    /// </summary>
    /// <param name="a">The first integer.</param>
    /// <param name="b">The second integer.</param>
    /// <returns>The GCD of the two integers.</returns>
    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    /// <summary>
    /// Gets the maximum weight among the items.
    /// </summary>
    /// <returns>The maximum weight.</returns>
    private int MaxWeight()
    {
        return _items.Max(i => i.weight);
    }
}
