namespace PowerPlaywright.Events
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// An event aggregator which allows for the publishing and subscription of events.
    /// </summary>
    internal interface IEventAggregator
    {
        /// <summary>
        /// Subscribe to an event.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="handler">The event handler.</param>
        void Subscribe<TEvent>(Func<TEvent, Task> handler);

        /// <summary>
        /// Publish an event.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PublishAsync<TEvent>(TEvent @event);
    }
}
