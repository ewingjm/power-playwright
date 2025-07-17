namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An entity form page.
    /// </summary>
    internal class SearchPage : ModelDrivenAppPage, ISearchPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public SearchPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }
    }
}