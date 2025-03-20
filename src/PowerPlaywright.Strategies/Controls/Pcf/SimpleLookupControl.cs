namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control strategy for the <see cref="ISimpleLookupControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 0, 470)]
    public class SimpleLookupControl : PcfControl, ISimpleLookupControl
    {
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator flyoutResults;
        private readonly ILocator flyoutNoRecordsText;
        private readonly ILocator selectedRecordListItem;
        private readonly ILocator selectedRecordText;
        private readonly ILocator selectedRecordDeleteButton;
        private readonly ILocator input;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLookupControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public SimpleLookupControl(IAppPage appPage, string name, IControl parent, ILogger<PcfGridControl> logger = null)
            : base(name, appPage, parent)
        {
            this.logger = logger;

            var flyoutRoot = this.Page.Locator($"div[data-id='{this.Name}.fieldControl|__flyoutRootNode_SimpleLookupControlFlyout']");
            this.flyoutNoRecordsText = flyoutRoot.Locator($"span[data-id='{this.Name}.fieldControl-LookupResultsDropdown_{this.Name}_No_Records_Text']");
            this.flyoutResults = flyoutRoot.GetByRole(AriaRole.Treeitem);
            this.selectedRecordListItem = this.Container.Locator($"ul[data-id*='{this.Name}.fieldControl-LookupResultsDropdown_{this.Name}_SelectedRecordList']").Locator("li").First;
            this.selectedRecordText = this.selectedRecordListItem.Locator($"div[data-id*='{this.Name}.fieldControl-LookupResultsDropdown_{this.Name}_selected_tag_text']");
            this.selectedRecordDeleteButton = this.selectedRecordListItem.Locator($"button[data-id*='{this.Name}.fieldControl-LookupResultsDropdown_{this.Name}_selected_tag_delete']");
            this.input = this.Container.Locator("input");
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
        public async Task SetValueAsync(string value)
        {
            await this.Page.WaitForAppIdleAsync();

            if (await this.selectedRecordListItem.IsVisibleAsync())
            {
                await this.selectedRecordListItem.HoverAsync();
                await this.selectedRecordDeleteButton.ClickAsync();
            }

            await this.input.ScrollIntoViewIfNeededAsync();
            await this.input.FillAsync(value);

            var flyoutResult = this.flyoutResults.GetByText(value);
            await flyoutResult.Or(this.flyoutNoRecordsText).WaitForAsync();

            if (!await flyoutResult.IsVisibleAsync())
            {
                throw new NotFoundException($"No records found in the {this.Name} lookup with search: {value}.");
            }

            await flyoutResult.ClickAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.SimpleLookupControl|{this.Name}.fieldControl|']");
        }
    }
}