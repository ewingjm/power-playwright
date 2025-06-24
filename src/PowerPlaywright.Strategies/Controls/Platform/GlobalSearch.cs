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
    using System;
    using System.Data;
    using System.Threading.Tasks;
    using System.Transactions;
    using System.Xml.Linq;

    /// <summary>
    /// A global searchText control.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class GlobalSearch : Control, IGlobalSearch
    {
        private const string RootLocator = "div[data-id='topBar']";
        private const string SearchFlyout = "id-globalSearchFlyout-1";

        private readonly IPageFactory pageFactory;
        private readonly ILocator clearSearchButton;
        private readonly ILocator search;
        private readonly ILocator searchFlyout;

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
            this.clearSearchButton = Container.Locator("i[data-icon-name='Clear']");
            this.searchFlyout = this.Page.Locator($"#{SearchFlyout}");
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator(RootLocator);
        }

        /// <inheritdoc/>
        public async Task SuggestAsync(string searchText, bool performSearch = false)
        {
            await Search(searchText, performSearch);
        }

        /// <inheritdoc/>
        public async Task<TPage> SearchAsync<TPage>(string search) where TPage : IAppPage
        {
            await Search(search, true);
            return await this.pageFactory.CreateInstanceAsync<TPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task<TPage> OpenSuggestionAsync<TPage>(string searchText, int index) where TPage : IAppPage
        {
            await Page.WaitForAppIdleAsync();
            await Search(searchText);

            await searchFlyout.WaitForAsync();

            var resultsLocator = searchFlyout.GetByRole(AriaRole.Row);
            await resultsLocator.First.WaitForAsync();

            var results = await resultsLocator.AllAsync();

            if (index < 0 || index >= results.Count)
            {
                throw new PowerPlaywrightException($"Expected at least {index + 1} results, but found {results.Count}.");
            }

            var selectedResult = results[index];

            if (!await selectedResult.IsVisibleAsync())
            {
                throw new PowerPlaywrightException($"Search result at index {index} is not visible.");
            }

            await selectedResult.ClickAndWaitForAppIdleAsync();
            return await pageFactory.CreateInstanceAsync<TPage>(Page);
        }

        /// <inheritdoc/>
        public async Task<bool> HasSuggestedResultsAsync()
        {
            await Page.WaitForAppIdleAsync();
            var resultsContainer = searchFlyout.GetByRole(AriaRole.Grid);
            return await resultsContainer.IsVisibleAsync();
        }

        /// <summary>
        /// Performs the searchText.
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="performSearch"></param>
        /// <returns></returns>
        private async Task Search(string searchText, bool performSearch = false)
        {
            if (await clearSearchButton.IsVisibleAsync())
            {
                await clearSearchButton.ClickAndWaitForAppIdleAsync();
            }

            await this.search.FillAsync(searchText);

            if (performSearch)
            {
                await this.search.PressAsync("Enter");
            }

            await Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<ISearchPage> OpenSearchTabAsync(string searchTabLabel)
        {
            var tabHeader = Page.GetByRole(AriaRole.Tablist);
            var tabs = await tabHeader.GetByRole(AriaRole.Tab).AllAsync();

            foreach (var tab in tabs)
            {
                var name = await tab.GetAttributeAsync("name");
                var isSelected = await tab.GetAttributeAsync("aria-selected");
                if (name == searchTabLabel && isSelected == "false")
                {
                    await tab.ClickAsync();
                }
            }

            return await this.pageFactory.CreateInstanceAsync<ISearchPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenSearchTabResult(int index)
        {
            await Page.WaitForAppIdleAsync();
            var rows = await Page.GetByRole(AriaRole.Row).AllAsync();

            foreach (var row in rows)
            {
                var rowIndexAttr = await row.GetAttributeAsync("row-index");
                if (int.TryParse(rowIndexAttr, out var rowIndex) && rowIndex == index)
                {
                    await row.DblClickAsync();
                    return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
                }
            }

            throw new InvalidOperationException($"Row with index {index} not found.");
        }
    }
}