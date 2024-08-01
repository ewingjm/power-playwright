namespace PowerPlaywright.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Notifications;

    /// <summary>
    /// A base class for control strategy resolvers that initialise on app load.
    /// </summary>
    internal abstract class AppControlStrategyResolver : IControlStrategyResolver, INotificationHandler<AppInitializedNotification>
    {
        private readonly IMediator mediator;

        private bool isReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="logger">The logger.</param>
        public AppControlStrategyResolver(IMediator mediator, ILogger logger)
        {
            this.mediator = mediator;
            this.Logger = logger;
        }

        /// <inheritdoc/>
        public event EventHandler ReadyStateChanged;

        /// <inheritdoc/>
        public bool IsReady => this.isReady;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task Handle(AppInitializedNotification notification, CancellationToken cancellationToken)
        {
            await this.Initialise(notification);
            this.isReady = true;

            this.ReadyStateChanged.Invoke(this, new EventArgs());

            await this.mediator.Publish(new ControlStrategyResolverReadyNotification(this), cancellationToken);
        }

        /// <inheritdoc/>
        public abstract bool IsResolvable(Type controlType);

        /// <inheritdoc/>
        public abstract Type Resolve(Type controlType, IEnumerable<Type> strategyTypes);

        /// <summary>
        /// Initialise the control strategy resolver.
        /// </summary>
        /// <param name="notification">The event.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task Initialise(AppInitializedNotification notification);
    }
}
