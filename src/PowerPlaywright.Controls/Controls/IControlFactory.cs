namespace PowerPlaywright.Model.Controls
{
    using Microsoft.Playwright;

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
        /// <param name="name">The control name.</param>
        /// <returns>The concrete implementation.</returns>
        TControl CreateInstance<TControl>(IPage page, string name = null)
            where TControl : IControl;
    }
}