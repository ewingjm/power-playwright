namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// A global search control.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class GlobalSearch : Control, IGlobalSearch
    {
        private const string RootLocator = "div[data-id='topBar']";
        private const string SearchFlyout = "id-globalSearchFlyout-1";

        private readonly IPageFactory pageFactory;
        private readonly ILocator search;
        private readonly ILocator searchFlyout;
        private readonly ILocator resultsContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMap"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="pageFactory">The page factory.</param>
        public GlobalSearch(IAppPage appPage, IPageFactory pageFactory)
            : base(appPage)
        {
            this.pageFactory = pageFactory;

            this.search = this.Container.GetByRole(AriaRole.Searchbox, new LocatorGetByRoleOptions
            {
                Name = "Search"
            });

            this.searchFlyout = this.Page.Locator("[id='id-globalSearchFlyout-1']");
            this.resultsContainer = searchFlyout.GetByRole(AriaRole.Grid);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator(RootLocator);
        }

        /// <inheritdoc/>
        public async Task SearchAsync(string searchText)
        {
            await this.search.FillAsync(searchText);
            await Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<TPage> OpenSearchResultAsync<TPage>(int index) where TPage : IModelDrivenAppPage
        {
            //if (await IsFlyOutVisible())
            //{
            //}
            //else
            //{
            //    throw new PowerPlaywrightException($"No search results were found in the global search");
            //}

            return default;
        }

        /// <inheritdoc/>
        public async Task<bool> HasResultsAsync()
        {
            if (await searchFlyout.IsVisibleAsync())
            {
                return await resultsContainer.IsVisibleAsync();
            }

            return false;
        }

        /// <summary>
        /// Indictates if the search results are visible.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsFlyOutVisible()
        {
            return await searchFlyout.IsVisibleAsync();
        }
    }
}