namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Extensions;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;

    /// <summary>
    /// A strategy resolver for PCF controls.
    /// </summary>
    internal class PcfControlStrategyResolver : AppControlStrategyResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PcfControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="environmentInfoProvider">The environment info provider.</param>
        /// <param name="logger">The logger.</param>
        public PcfControlStrategyResolver(IEnvironmentInfoProvider environmentInfoProvider, ILogger<PcfControlStrategyResolver> logger = null)
            : base(environmentInfoProvider, logger)
        {
        }

        /// <inheritdoc/>
        public override bool IsResolvable(Type controlType)
        {
            if (controlType is null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            return controlType.GetCustomAttribute<PcfControlAttribute>() != null;
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

            if (this.EnvironmentInfoProvider.ControlVersions is null)
            {
                throw new PowerPlaywrightException("The environment's control versions could not be determined.");
            }

            if (controlType.GetCustomAttribute<PcfControlAttribute>() is PcfControlAttribute control)
            {
                return strategyTypes
                    .Where(s => (controlType.IsAssignableFrom(s) || (controlType.IsGenericTypeDefinition && controlType.IsGenericAssignableFrom(s))) && !s.IsAbstract && s.IsClass && s.IsVisible)
                    .Select(s =>
                        new
                        {
                            Type = s,
                            s.GetCustomAttribute<PcfControlStrategyAttribute>().Version,
                        })
                    .Where(s => s.Version <= this.EnvironmentInfoProvider.ControlVersions[control.Name])
                    .OrderByDescending(s => s.Version)
                    .FirstOrDefault()?.Type;
            }

            throw new PowerPlaywrightException($"No supported attributes were found for control type {controlType.Name}. {nameof(PcfControlStrategyResolver)} resolver is unable to resolve the control strategy.");
        }
    }
}