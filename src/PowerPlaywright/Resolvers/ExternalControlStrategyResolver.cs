namespace PowerPlaywright.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using PowerPlaywright.Framework.Controls.External.Attributes;

    /// <summary>
    /// Resolves control strategies for external controls (<see cref="ExternalControlAttribute"/>).
    /// </summary>
    internal class ExternalControlStrategyResolver : IControlStrategyResolver
    {
        /// <inheritdoc/>
        public event EventHandler<ResolverReadyEventArgs> OnReady;

        /// <inheritdoc/>
        public bool IsReady => true;

        /// <inheritdoc/>
        public bool IsResolvable(Type t)
        {
            if (t is null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return t.GetCustomAttribute<ExternalControlAttribute>() != null;
        }

        /// <inheritdoc/>
        public Type Resolve(Type controlType, IEnumerable<Type> strategyTypes)
        {
            if (controlType is null)
            {
                throw new ArgumentNullException(nameof(controlType));
            }

            if (strategyTypes is null)
            {
                throw new ArgumentNullException(nameof(strategyTypes));
            }

            return strategyTypes
                .Where(s => controlType.IsAssignableFrom(s) && !s.IsAbstract && s.IsClass && s.IsVisible)
                .OrderByDescending(s => s.GetCustomAttribute<ExternalControlStrategyAttribute>().Version)
                .FirstOrDefault();
        }
    }
}