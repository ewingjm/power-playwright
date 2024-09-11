namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;

    /// <summary>
    /// A custom page.
    /// </summary>
    internal class CustomPage : ModelDrivenAppPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        public CustomPage(IPage page, IPageFactory pageFactory, IControlFactory controlFactory)
            : base(page, pageFactory, controlFactory)
        {
            this.Container = page.Locator("#mainContent");
        }

        /// <summary>
        /// Gets the page container.
        /// </summary>
        protected ILocator Container { get; }
    }
}
