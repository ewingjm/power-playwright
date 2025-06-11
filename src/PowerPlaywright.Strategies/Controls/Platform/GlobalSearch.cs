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
    using System.Data;
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
        /// Initializes a new instance of the <see cref="GlobalSearch"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="pageFactory">The page factory.</param>
        public GlobalSearch(IAppPage appPage, IPageFactory pageFactory)
            : base(appPage)
        {
            this.pageFactory = pageFactory;
            this.search = this.Container.GetByRole(AriaRole.Searchbox, new LocatorGetByRoleOptions { Name = "Search" });

            this.searchFlyout = this.Page.Locator($"#{SearchFlyout}");
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
            await Search(searchText);
        }

        /// <inheritdoc/>
        public async Task<TPage> OpenSearchResultAsync<TPage>(string search, int index) where TPage : IModelDrivenAppPage
        {
            await Search(search);

            var results = await this.searchFlyout.GetByRole(AriaRole.Button).AllAsync();

            await results[index].ClickAndWaitForAppIdleAsync();

            return await this.pageFactory.CreateInstanceAsync<TPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task<bool> HasResultsAsync()
        {
            return await resultsContainer.IsVisibleAsync();
        }

        /// <summary>
        /// Performs the search.
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        private async Task Search(string searchText)
        {
            await this.search.ClearAsync();
            await this.search.FillAsync(searchText);
            await Page.WaitForAppIdleAsync();
        }
    }
}