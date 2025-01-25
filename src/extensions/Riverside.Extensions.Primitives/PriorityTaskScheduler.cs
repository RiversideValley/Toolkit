using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Riverside.Extensions
{
    /// <summary>
    /// Provides a priority-based task scheduler to control the execution of tasks based on their priority.
    /// </summary>
    public class PriorityTaskScheduler(int maxConcurrentTasks)
    {
        private readonly ConcurrentDictionary<int, Queue<Func<Task>>> _taskQueues = new();
        private readonly SemaphoreSlim _semaphore = new(maxConcurrentTasks, maxConcurrentTasks);
        private readonly object _lock = new();

        /// <summary>
        /// Enqueues a task with the specified priority.
        /// </summary>
        /// <param name="task">The task to enqueue.</param>
        /// <param name="priority">The priority of the task.</param>
        public void EnqueueTask(Func<Task> task, int priority)
        {
            lock (_lock)
            {
                if (!_taskQueues.ContainsKey(priority))
                {
                    _taskQueues[priority] = new Queue<Func<Task>>();
                }

                _taskQueues[priority].Enqueue(task);
            }

            _ = Task.Run(ProcessTasks);
        }

        /// <summary>
        /// Processes the tasks in the queue based on their priority.
        /// </summary>
        private async Task ProcessTasks()
        {
            await _semaphore.WaitAsync();

            try
            {
                Func<Task> taskToExecute = null;

                lock (_lock)
                {
                    foreach (int priority in _taskQueues.Keys.OrderByDescending(p => p))
                    {
                        if (_taskQueues[priority].Count > 0)
                        {
                            taskToExecute = _taskQueues[priority].Dequeue();
                            if (_taskQueues[priority].Count == 0)
                            {
                                _ = _taskQueues.TryRemove(priority, out _);
                            }
                            break;
                        }
                    }
                }

                if (taskToExecute != null)
                {
                    await taskToExecute();
                }
            }
            finally
            {
                _ = _semaphore.Release();
            }
        }
    }
}
