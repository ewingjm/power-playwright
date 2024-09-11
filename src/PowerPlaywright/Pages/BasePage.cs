namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A base class for all pages.
    /// </summary>
    public abstract class BasePage : IBasePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasePage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public BasePage(IPage page, IControlFactory controlFactory)
        {
            this.Page = page;
            this.ControlFactory = controlFactory;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        public IPage Page { get; }

        /// <summary>
        /// Gets the control factory.
        /// </summary>
        protected IControlFactory ControlFactory { get; }
    }
}
