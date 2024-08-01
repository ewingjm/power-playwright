namespace PowerPlaywright.Model.Controls
{
    using Microsoft.Playwright;

    /// <summary>
    /// An interface representing a control.
    /// </summary>
    public interface IControl
    {
        /// <summary>
        /// Gets the parent control.
        /// </summary>
        IControl Parent { get; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        ILocator Container { get; }
    }
}