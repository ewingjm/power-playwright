namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="ISimpleLookupControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 0, 470)]
    public class SimpleLookupControl : PcfControlInternal, ISimpleLookupControl
    {
        private readonly IControlFactory controlFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator flyoutRoot;
        private readonly ILocator resultsRoot;
        private readonly ILocator results;
        private readonly ILocator noRecordsText;
        private readonly ILocator newButton;
        private readonly ILocator selectedRecordListItem;
        private readonly ILocator selectedRecordText;
        private readonly ILocator selectedRecordDeleteButton;
        private readonly ILocator input;
        private readonly ILocator searchButton;
        private readonly ILocator itemContainer;
        private readonly ILocator itemInfoContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLookupControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="infoProvider"> The info provider.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public SimpleLookupControl(IAppPage appPage, string name, IEnvironmentInfoProvider infoProvider, IControlFactory controlFactory, IControl parent, ILogger<PcfGridControl> logger = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.controlFactory = controlFactory;
            this.logger = logger;

            this.flyoutRoot = this.Page.Locator($"div[data-id='{this.Name}.fieldControl|__flyoutRootNode_SimpleLookupControlFlyout']");
            this.resultsRoot = this.Page.GetByRole(AriaRole.Tree, new PageGetByRoleOptions { Name = "Lookup results", Exact = true });
            this.results = this.resultsRoot.GetByRole(AriaRole.Treeitem);
            this.noRecordsText = this.flyoutRoot.Locator($"span[data-id*='_No_Records_Text']");
            this.newButton = this.flyoutRoot.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "New", Exact = true });
            this.selectedRecordListItem = this.Container.Locator($"ul[data-id*='_SelectedRecordList']").Or(this.Container.Locator("div[id*='_RecordList']").GetByRole(AriaRole.List)).GetByRole(AriaRole.Listitem).First;
            this.selectedRecordText = this.selectedRecordListItem.Locator($"div[data-id*='_selected_tag_text']");
            this.selectedRecordDeleteButton = this.selectedRecordListItem.Locator($"button[data-id*='_selected_tag_delete']");
            this.input = this.Container.Locator("input");
            this.searchButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Search" });
            this.itemContainer = this.resultsRoot.GetByRole(AriaRole.Treeitem);
            this.itemInfoContainer = this.itemContainer.Locator($"div[data-id='{this.Name}.fieldControl-LookupResultsDropdown_{this.Name}_infoContainer']");
        }

        /// <inheritdoc/>
        public async Task<List<List<string>>> GetSearchResultsAsync(string search = "")
        {
            var itemInfo = new List<List<string>>();

            await this.Page.WaitForAppIdleAsync();

            if (!string.IsNullOrEmpty(search))
            {
                await this.FillAndWaitForResults(search);
                if (await this.noRecordsText.IsVisibleAsync())
                {
                    return itemInfo;
                }

                await this.itemInfoContainer.WaitForAsync();
            }
            else
            {
                await this.ClickSearchAndWaitForResults();
            }

            foreach (var item in await this.itemInfoContainer.AllAsync())
            {
                var spanTexts = (await item.Locator("span[data-id]").AllInnerTextsAsync())
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();

                if (spanTexts.Any())
                {
                    itemInfo.Add(spanTexts);
                }
            }

            return itemInfo;
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            if (!await this.selectedRecordListItem.IsVisibleAsync())
            {
                return null;
            }

            return await this.selectedRecordText.InnerTextAsync();
        }

        /// <inheritdoc/>
        public async Task<IQuickCreateForm> NewViaQuickCreateAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.ClearExistingValue();
            await this.input.ClickAndWaitForAppIdleAsync();
            await this.newButton.ClickAndWaitForAppIdleAsync();

            return this.controlFactory.CreateCachedInstance<IQuickCreateForm>(this.AppPage, parent: this);
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.ClearExistingValue();
            await this.input.ScrollIntoViewIfNeededAsync();

            if (value is null)
            {
                return;
            }

            await this.input.FillAsync(value);
            await this.resultsRoot.Or(this.noRecordsText).WaitForAsync();
            await this.Page.WaitForAppIdleAsync();

            var flyoutResult = this.results.GetByText(value, new LocatorGetByTextOptions { Exact = true }).First;

            if (!await flyoutResult.IsVisibleAsync())
            {
                flyoutResult = this.results
                    .GetByText(new Regex($"{Regex.Escape(value)}( \\(Offline\\)| \\(Online\\))?"))
                    .First;

                if (!await flyoutResult.IsVisibleAsync())
                {
                    throw new NotFoundException($"Unable to find a value in the {this.Name} lookup with search: {value}.");
                }
            }

            await flyoutResult.ClickAndWaitForAppIdleAsync();
        }

        private async Task ClearExistingValue()
        {
            if (await this.selectedRecordListItem.IsVisibleAsync())
            {
                await this.selectedRecordListItem.HoverAsync();
                await this.selectedRecordDeleteButton.ClickAsync();
            }
        }

        private async Task FillAndWaitForResults(string search)
        {
            await this.input.ClickAndWaitForAppIdleAsync();
            await this.input.FillAsync(search);
            await this.resultsRoot.Or(this.noRecordsText).WaitForAsync();
        }

        private async Task ClickSearchAndWaitForResults()
        {
            await this.searchButton.ClickAsync();
            await this.resultsRoot.And(this.flyoutRoot).IsVisibleAsync();
        }
    }
}