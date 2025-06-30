namespace PowerPlaywright.Framework
{
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Pages;
    using System;

    /// <summary>
    /// Instantiates a concrete implementation of a given control.
    /// </summary>
    public interface IControlFactory
    {
        /// <summary>
        /// Gets the type of the interface that is redirected to when a control type is requested.
        /// </summary>
        /// <typeparam name="TControl">The control interface.</typeparam>
        /// <returns>The type of control redirected to.</returns>
        Type GetRedirectedType<TControl>()
            where TControl : IControl;

        /// <summary>
        /// Instantiates a concrete implementation of <typeparamref name="TControl"/>.
        /// </summary>
        /// <typeparam name="TControl">The control interface.</typeparam>
        /// <param name="appPage">The page.</param>
        /// <param name="name">An optional control name.</param>
        /// <param name="parent">An optional parent control.</param>
        /// <returns>The concrete implementation.</returns>
        TControl CreateInstance<TControl>(IAppPage appPage, string name = null, IControl parent = null)
            where TControl : IControl;
    }
}