namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework;

    /// <summary>
    /// A base class for control strategy resolvers that initialise on app load.
    /// </summary>
    internal abstract class AppControlStrategyResolver : IControlStrategyResolver
    {
        private bool isReady;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="environmentInfoProvider">The environment info provider.</param>
        /// <param name="logger">The logger.</param>
        public AppControlStrategyResolver(IEnvironmentInfoProvider environmentInfoProvider, ILogger logger)
        {
            this.EnvironmentInfoProvider = environmentInfoProvider;
            this.Logger = logger;
            this.EnvironmentInfoProvider.OnReady += this.EnvironmentInfoProvider_OnReady;
            this.IsReady = this.EnvironmentInfoProvider.IsReady;
        }

        /// <inheritdoc/>
        public event EventHandler OnReady;

        /// <inheritdoc/>
        public bool IsReady
        {
            get
            {
                return this.isReady;
            }

            protected set
            {
                this.isReady = value;

                if (value)
                {
                    this.OnReady?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the environment info provider.
        /// </summary>
        protected IEnvironmentInfoProvider EnvironmentInfoProvider { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <inheritdoc/>
        public abstract bool IsResolvable(Type controlType);

        /// <inheritdoc/>
        public abstract Type Resolve(Type controlType, IEnumerable<Type> strategyTypes);

        private void EnvironmentInfoProvider_OnReady(object sender, EventArgs e)
        {
            this.IsReady = true;
        }
    }
}