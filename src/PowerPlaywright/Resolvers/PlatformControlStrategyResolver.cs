namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Logging;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// A strategy resolver for platform controls.
    /// </summary>
    internal class PlatformControlStrategyResolver : AppControlStrategyResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformControlStrategyResolver"/> class.
        /// </summary>
        /// <param name="environmentInfoProvider">The environment info provider.</param>
        /// <param name="logger">The logger.</param>
        public PlatformControlStrategyResolver(IEnvironmentInfoProvider environmentInfoProvider, ILogger<PlatformControlStrategyResolver> logger = null)
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

            if (this.EnvironmentInfoProvider.PlatformVersion is null)
            {
                throw new PowerPlaywrightException("The environment's platform version could not be determined.");
            }

            if (controlType.GetCustomAttribute<PlatformControlAttribute>() is PlatformControlAttribute control)
            {
                return strategyTypes
                    .Where(s =>
                    {
                        if (s.IsAbstract || !s.IsClass || !s.IsVisible)
                        {
                            return false;
                        }

                        if (controlType.IsAssignableFrom(s))
                        {
                            return true;
                        }

                        if (controlType.IsGenericTypeDefinition)
                        {
                            return s.GetInterfaces().Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == controlType);
                        }

                        return false;
                    })
                    .Select(s =>
                        new
                        {
                            Type = s,
                            s.GetCustomAttribute<PlatformControlStrategyAttribute>().Version,
                        })
                    .Where(s => s.Version <= this.EnvironmentInfoProvider.PlatformVersion)
                    .OrderByDescending(s => s.Version)
                    .FirstOrDefault()?.Type;
            }

            throw new PowerPlaywrightException($"No supported attributes were found for control type {controlType.Name}. {nameof(PlatformControlStrategyResolver)} resolver is unable to resolve the control strategy.");
        }
    }
}