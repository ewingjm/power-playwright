namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// A strategy resolver for platform controls.
    /// </summary>
    internal class PlatformControlStrategyResolver : AppControlStrategyResolver
    {
        private Version platformVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public PlatformControlStrategyResolver(ILogger<PlatformControlStrategyResolver> logger = null)
            : base(logger)
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
            if (controlType is null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            if (strategyTypes is null)
            {
                throw new ArgumentNullException(nameof(strategyTypes));
            }

            if (this.platformVersion is null)
            {
                throw new PowerPlaywrightException($"The {nameof(PcfControlStrategyResolver)} must be initialised before it can resolve controls");
            }

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

            throw new PowerPlaywrightException($"No supported attributes were found for control type {controlType.Name}. {nameof(PlatformControlStrategyResolver)} resolver is unable to resolve the control strategy.");
        }

        /// <inheritdoc/>
        protected override async Task InitialiseResolverAsync(IPage page)
        {
            this.platformVersion = await this.GetPlatformVersionAsync(page);
        }

        private async Task<Version> GetPlatformVersionAsync(IPage page)
        {
            return new Version(await page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()"));
        }
    }
}
