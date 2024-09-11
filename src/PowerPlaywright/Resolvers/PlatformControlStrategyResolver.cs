namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Events;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Events;

    /// <summary>
    /// A strategy resolver for platform controls.
    /// </summary>
    internal class PlatformControlStrategyResolver : AppControlStrategyResolver
    {
        private Version platformVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        public PlatformControlStrategyResolver(IEventAggregator eventAggregator, ILogger<PlatformControlStrategyResolver> logger = null)
            : base(eventAggregator, logger)
        {
        }

        /// <inheritdoc/>
        public override bool IsResolvable(Type controlType)
        {
            if (controlType is null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            return controlType.GetCustomAttribute<PlatformControlAttribute>() != null;
        }

        /// <inheritdoc/>
        public override Type Resolve(Type controlType, IEnumerable<Type> strategyTypes)
        {
            if (controlType.GetCustomAttribute<PlatformControlAttribute>() is PlatformControlAttribute control)
            {
                return strategyTypes
                    .Where(s => controlType.IsAssignableFrom(s) && !s.IsAbstract && s.IsClass && s.IsVisible)
                    .Select(s =>
                        new
                        {
                            Type = s,
                            s.GetCustomAttribute<PlatformControlStrategyAttribute>().Version,
                        })
                    .Where(s => s.Version <= this.platformVersion)
                    .OrderByDescending(s => s.Version)
                    .FirstOrDefault()?.Type;
            }

            throw new NotSupportedException($"No supported attributes were found for control type {controlType.Name}. {nameof(PlatformControlStrategyResolver)} resolver is unable to resolve the control strategy.");
        }

        /// <inheritdoc/>
        protected override async Task Initialise(AppInitializedEvent @event)
        {
            this.platformVersion = await this.GetPlatformVersionAsync(@event.HomePage.Page);
        }

        private async Task<Version> GetPlatformVersionAsync(IPage page)
        {
            return new Version(await page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()"));
        }
    }
}
