namespace Riverside.Extensions.Policies;

/// <summary>
/// Provides a timeout policy to handle operations that exceed a specified time limit.
/// </summary>
public static class TimeoutPolicy
{
    /// <summary>
    /// Executes the specified operation with a timeout.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the operation.</returns>
    /// <exception cref="TimeoutException">Thrown when the operation exceeds the specified timeout.</exception>
    public static async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, TimeSpan timeout)
    {
        using CancellationTokenSource cts = new();
        Task timeoutTask = Task.Delay(timeout, cts.Token);
        Task<T> operationTask = operation(cts.Token);

        Task completedTask = await Task.WhenAny(operationTask, timeoutTask);

        if (completedTask == timeoutTask)
        {
            throw new TimeoutException("The operation has timed out.");
        }

        cts.Cancel(); // Cancel the timeout task
        return await operationTask; // Await the operation task to propagate any exceptions
    }
}
