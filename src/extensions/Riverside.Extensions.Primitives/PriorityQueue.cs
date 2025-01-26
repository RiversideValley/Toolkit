namespace Riverside.Extensions
{
    /// <summary>
    /// Represents a priority queue where elements are dequeued based on their priority.
    /// </summary>
    /// <typeparam name="T">The type of elements in the priority queue.</typeparam>
    public class PriorityQueue<T>
    {
        private readonly List<(T item, int priority)> _elements = [];

        /// <summary>
        /// Gets the number of elements in the priority queue.
        /// </summary>
        public int Count => _elements.Count;

        /// <summary>
        /// Enqueues an item with the specified priority.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        /// <param name="priority">The priority of the item.</param>
        public void Enqueue(T item, int priority)
        {
            _elements.Add((item, priority));
            _elements.Sort((x, y) => y.priority.CompareTo(x.priority)); // Sort by priority in descending order
        }

        /// <summary>
        /// Dequeues the item with the highest priority.
        /// </summary>
        /// <returns>The item with the highest priority.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the priority queue is empty.</exception>
        public T Dequeue()
        {
            if (_elements.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            T item = _elements[0].item;
            _elements.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Peeks at the item with the highest priority without removing it.
        /// </summary>
        /// <returns>The item with the highest priority.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the priority queue is empty.</exception>
        public T Peek()
        {
            return _elements.Count == 0 ? throw new InvalidOperationException("The priority queue is empty.") : _elements[0].item;
        }
    }
}
