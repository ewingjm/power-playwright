namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

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
        private readonly ILocator resultsLocator;

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
            this.clearSearchButton = this.Container.Locator("i[data-icon-name='Clear']");
            this.searchFlyout = this.Page.Locator($"#{SearchFlyout}");
            this.resultsLocator = this.searchFlyout.GetByRole(AriaRole.Row).Locator("button[data-is-focusable]");
        }

        /// <inheritdoc/>
        public async Task SuggestAsync(string searchText)
        {
            await this.EnterSearchText(searchText);
        }

        /// <inheritdoc/>
        public async Task<TPage> SearchAsync<TPage>(string search)
            where TPage : IAppPage
        {
            await this.Search(search);
            return await this.pageFactory.CreateInstanceAsync<TPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task<TPage> OpenSuggestionAsync<TPage>(string searchText, int index)
            where TPage : IAppPage
        {
            await this.Page.WaitForAppIdleAsync();
            await this.EnterSearchText(searchText);

            await this.searchFlyout.WaitForAsync();
            await this.resultsLocator.First.WaitForAsync();

            var results = await this.resultsLocator.AllAsync();

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
            return await this.pageFactory.CreateInstanceAsync<TPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task<bool> HasSuggestedResultsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            try
            {
                var resultsContainer = this.searchFlyout.GetByRole(AriaRole.Grid);
                await resultsContainer.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000, State = WaitForSelectorState.Visible });
                return await resultsContainer.IsVisibleAsync();
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> SearchAndOpenResultAsync(string search, string table, int index)
        {
            await this.Search($"{table} {search}");

            await this.Page.WaitForAppIdleAsync();
            var rows = await this.Page.GetByRole(AriaRole.Row).AllAsync();

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

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator(RootLocator);
        }

        /// <summary>
        /// Performs the searchText.
        /// </summary>
        /// <param name="searchText">The search Text to enter.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task Search(string searchText)
        {
            await this.EnterSearchText(searchText);
            await this.search.PressAsync("Enter");
            await this.Page.WaitForAppIdleAsync();
        }

        private async Task EnterSearchText(string searchText)
        {
            if (await this.clearSearchButton.IsVisibleAsync())
            {
                await this.clearSearchButton.ClickAndWaitForAppIdleAsync();
            }

            await this.search.FillAsync(searchText);
        }
    }
}