namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Events;
    using PowerPlaywright.Model.Events;
    using PowerPlaywright.Notifications;

    /// <summary>
    /// A base class for control strategy resolvers that initialise on app load.
    /// </summary>
    internal abstract class AppControlStrategyResolver : IControlStrategyResolver
    {
        private readonly IEventAggregator eventAggregator;

        private bool isReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        public AppControlStrategyResolver(IEventAggregator eventAggregator, ILogger logger)
        {
            this.eventAggregator = eventAggregator;
            this.Logger = logger;

            this.eventAggregator.Subscribe<AppInitializedEvent>(this.InitialiseInternal);
        }

        /// <inheritdoc/>
        public bool IsReady => this.isReady;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public abstract bool IsResolvable(Type controlType);

        /// <inheritdoc/>
        public abstract Type Resolve(Type controlType, IEnumerable<Type> strategyTypes);

        /// <summary>
        /// Initialise the control strategy resolver.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task Initialise(AppInitializedEvent @event);

        private async Task InitialiseInternal(AppInitializedEvent @event)
        {
            await this.Initialise(@event);
            this.isReady = true;

            await this.eventAggregator.PublishAsync(new ResolverReadyEvent(this));
        }
    }
}
