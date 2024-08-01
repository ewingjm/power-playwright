namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using PowerPlaywright.Model.Events;

    /// <summary>
    /// An event aggregator which allows for the publishing and subscription of events.
    /// </summary>
    internal class EventAggregator : IEventAggregator
    {
        private readonly IDictionary<Type, List<object>> subscribers;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAggregator"/> class.
        /// </summary>
        public EventAggregator()
        {
            this.subscribers = new Dictionary<Type, List<object>>();
        }

        /// <inheritdoc/>
        public void Subscribe<TEvent>(Func<TEvent, Task> handler)
        {
            if (!this.subscribers.TryGetValue(typeof(TEvent), out var actions))
            {
                actions = new List<object>();
                this.subscribers[typeof(TEvent)] = actions;
            }

            actions.Add(handler);
        }

        /// <inheritdoc/>
        public async Task PublishAsync<TEvent>(TEvent @event)
        {
            if (!this.subscribers.TryGetValue(@event.GetType(), out var actions))
            {
                return;
            }

            var tasks = actions
                .OfType<Func<TEvent, Task>>()
                .Select(action => action(@event));

            await Task.WhenAll(tasks);
        }
    }
}
