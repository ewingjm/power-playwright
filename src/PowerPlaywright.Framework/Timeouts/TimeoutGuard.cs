namespace PowerPlaywright.Framework.Timeouts
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper for enforcing a flat timeout for UI loops.
    /// </summary>
    public static class TimeoutGuard
    {
        /// <summary>
        /// The default timeout used across the library for UI-driven loops. This is configurable.
        /// </summary>
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Executes the provided asynchronous function and throws a <see cref="TimeoutException"/> if it does not complete within the configured timeout.
        /// </summary>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="timeout">Optional timeout to override the configured default.</param>
        /// <param name="message">Optional message to include in the thrown TimeoutException.</param>
        /// <returns>A task that completes when the operation completes or throws on timeout.</returns>
        public static async Task ExecuteWithTimeoutAsync(Func<Task> operation, TimeSpan? timeout = null, string message = null)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var operationTask = operation();
            var delay = Task.Delay((int)(timeout ?? DefaultTimeout).TotalMilliseconds);

            var winner = await Task.WhenAny(operationTask, delay).ConfigureAwait(false);

            if (winner != operationTask)
            {
                throw new TimeoutException(message ?? $"Operation timed out after {timeout ?? DefaultTimeout}.");
            }

            // Propagate exceptions/cancellation from the operation.
            await operationTask.ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the provided asynchronous function and returns its result. Throws a <see cref="TimeoutException"/> if it does not complete within the configured timeout.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="operation">The asynchronous operation to execute.</param>
        /// <param name="timeout">Optional timeout to override the configured default.</param>
        /// <param name="message">Optional message to include in the thrown TimeoutException.</param>
        /// <returns>The operation result if it completes before the timeout.</returns>
        public static async Task<T> ExecuteWithTimeoutAsync<T>(Func<Task<T>> operation, TimeSpan? timeout = null, string message = null)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var operationTask = operation();
            var delay = Task.Delay((int)(timeout ?? DefaultTimeout).TotalMilliseconds);

            var winner = await Task.WhenAny(operationTask, delay).ConfigureAwait(false);

            if (winner != operationTask)
            {
                throw new TimeoutException(message ?? $"Operation timed out after {timeout ?? DefaultTimeout}.");
            }

            // Propagate exceptions/cancellation from the operation and return result.
            return await operationTask.ConfigureAwait(false);
        }
    }
}
