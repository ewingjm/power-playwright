namespace PowerPlaywright.Framework.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Extensions to the <see cref="IControlFactory"/> interface.
    /// </summary>
    public static class IControlFactoryExtensions
    {
        private static readonly ConcurrentDictionary<(IAppPage, Type, string), IControl> ControlCache;

        static IControlFactoryExtensions()
        {
            ControlCache = new ConcurrentDictionary<(IAppPage, Type, string), IControl>();
        }

        /// <summary>
        /// Creates a cached instance of a control.
        /// </summary>
        /// <typeparam name="TControl">The type of control.</typeparam>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name.</param>
        /// <param name="parent">The parent.</param>
        /// <returns>A new control or a cached instance if already created.</returns>
        public static TControl CreateCachedInstance<TControl>(this IControlFactory controlFactory, IAppPage appPage, string name = null, IControl parent = null)
            where TControl : IControl
        {
            var cacheKey = (appPage, typeof(TControl), name);

            if (ControlCache.TryGetValue(cacheKey, out var control))
            {
                return (TControl)control;
            }

            control = controlFactory.CreateInstance<TControl>(appPage, name, parent);

            ControlCache.TryAdd(cacheKey, control);

            return (TControl)control;
        }
    }
}
