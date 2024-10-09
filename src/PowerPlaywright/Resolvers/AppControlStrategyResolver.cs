namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;

    /// <summary>
    /// A base class for control strategy resolvers that initialise on app load.
    /// </summary>
    internal abstract class AppControlStrategyResolver : IControlStrategyResolver, IAppLoadInitializable
    {
        private bool isReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public AppControlStrategyResolver(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <inheritdoc/>
        public event EventHandler<ResolverReadyEventArgs> OnReady;

        /// <inheritdoc/>
        public bool IsReady => this.isReady;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public async Task InitializeAsync(IPage page)
        {
            if (this.IsReady)
            {
                return;
            }

            await this.InitialiseResolverAsync(page);

            this.isReady = true;
            this.OnReady?.Invoke(this, new ResolverReadyEventArgs(this));
        }

        /// <inheritdoc/>
        public abstract bool IsResolvable(Type controlType);

        /// <inheritdoc/>
        public abstract Type Resolve(Type controlType, IEnumerable<Type> strategyTypes);

        /// <summary>
        /// Initialise the control strategy resolver.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected abstract Task InitialiseResolverAsync(IPage page);
    }
}
