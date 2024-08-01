namespace PowerPlaywright.Model.Controls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Resolves a control interface to a strategy.
    /// </summary>
    public interface IControlStrategyResolver
    {
        /// <summary>
        /// Gets a value indicating whether whether or not the strategy resolver is ready to resolve controls.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Resolve the control type to a specific strategy.
        /// </summary>
        /// <param name="controlType">The control type.</param>
        /// <param name="strategyTypes">The available strategy types.</param>
        /// <returns>The resolved strategy.</returns>
        Type Resolve(Type controlType, IEnumerable<Type> strategyTypes);

        /// <summary>
        /// Whether the control type can be resolved by this resolver.
        /// </summary>
        /// <param name="controlType">The control type.</param>
        /// <returns>True is the resolver can resolve controls of this type.</returns>
        bool IsResolvable(Type controlType);
    }
}