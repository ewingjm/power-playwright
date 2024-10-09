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
    public class SimpleLookupControl : Control, ISimpleLookupControl
    {
        private readonly string name;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator root;
        private readonly ILocator flyoutRoot;
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
            : base(appPage, parent)
        {
            this.name = name;
            this.logger = logger;

            this.root = this.Container.Locator($"div[data-lp-id*='MscrmControls.Containers.FieldSectionItem|{this.name}']");
            this.flyoutRoot = this.Page.Locator($"div[data-id='{this.name}.fieldControl|__flyoutRootNode_SimpleLookupControlFlyout']");
            this.flyoutNoRecordsText = this.flyoutRoot.Locator($"span[data-id='{this.name}.fieldControl-LookupResultsDropdown_{this.name}_No_Records_Text']");
            this.flyoutResults = this.flyoutRoot.GetByRole(AriaRole.Treeitem);
            this.selectedRecordListItem = this.Container.Locator($"ul[data-id*='{this.name}.fieldControl-LookupResultsDropdown_{this.name}_SelectedRecordList']").Locator("li").First;
            this.selectedRecordText = this.selectedRecordListItem.Locator($"div[data-id*='{this.name}.fieldControl-LookupResultsDropdown_{this.name}_selected_tag_text']");
            this.selectedRecordDeleteButton = this.selectedRecordListItem.Locator($"button[data-id*='{this.name}.fieldControl-LookupResultsDropdown_{this.name}_selected_tag_delete']");
            this.input = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        protected override ILocator Root => this.root;

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
                throw new NotFoundException($"No records found in the {this.name} lookup with search: {value}.");
            }

            await flyoutResult.ClickAsync();
        }
    }
}