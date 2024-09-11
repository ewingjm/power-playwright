namespace PowerPlaywright.Framework.Controls
{
    using Microsoft.Playwright;

    /// <summary>
    /// An interface representing a control.
    /// </summary>
    public interface IControl
    {
        /// <summary>
        /// Gets the parent control (if any).
        /// </summary>
        IControl Parent { get; }

        /// <summary>
        /// Gets the control container.
        /// </summary>
        ILocator Container { get; }
    }
}