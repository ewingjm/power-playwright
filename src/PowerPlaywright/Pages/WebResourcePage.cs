namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A web resource page.
    /// </summary>
    internal class WebResourcePage : ModelDrivenAppPage, IWebResourcePage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebResourcePage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        public WebResourcePage(IPage page, IPageFactory pageFactory, IControlFactory controlFactory)
            : base(page, pageFactory, controlFactory)
        {
        }
    }
}
