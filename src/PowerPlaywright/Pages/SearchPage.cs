namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using System.Threading.Tasks;

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

        private IGlobalSearch searchControl => this.GetControl<IGlobalSearch>();

        public async Task<ISearchPage> OpenTabAsync(string tabName)
        {
            return await searchControl.OpenSearchTabAsync(tabName);
        }

        public async Task<IEntityRecordPage> OpenResultAsync(int index)
        {
            return await searchControl.OpenSearchTabResult(index);
        }
    }
}