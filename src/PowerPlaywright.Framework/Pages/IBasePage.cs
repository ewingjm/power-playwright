namespace PowerPlaywright.Framework.Pages
{
    using Microsoft.Playwright;

    /// <summary>
    /// Represents a page.
    /// </summary>
    public interface IBasePage
    {
        /// <summary>
        /// Gets the Playwright <see cref="IPage"/> for the page.
        /// </summary>
        IPage Page { get; }
    }
}
