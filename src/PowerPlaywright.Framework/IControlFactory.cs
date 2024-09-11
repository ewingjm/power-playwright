namespace PowerPlaywright.Framework
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls;

    /// <summary>
    /// Instantiates a concrete implementation of a given control.
    /// </summary>
    public interface IControlFactory
    {
        /// <summary>
        /// Instantiates a concrete implementation of <typeparamref name="TControl"/>.
        /// </summary>
        /// <typeparam name="TControl">The control interface.</typeparam>
        /// <param name="page">The page.</param>
        /// <param name="name">An optional control name.</param>
        /// <param name="parent">An optional parent control.</param>
        /// <returns>The concrete implementation.</returns>
        TControl CreateInstance<TControl>(IPage page, string name = null, IControl parent = null)
            where TControl : IControl;
    }
}