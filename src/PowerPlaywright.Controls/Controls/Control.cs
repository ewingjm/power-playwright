namespace PowerPlaywright.Model.Controls
{
    using Microsoft.Playwright;

    /// <summary>
    /// A base class that all controls must inherit from.
    /// </summary>
    public abstract class Control : IControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Control"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        protected Control(IPage page)
        {
            this.Page = page;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>The page.</value>
        protected IPage Page { get; }
    }
}