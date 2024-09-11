namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
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
    /// A control strategy for the <see cref="IPcfGridControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 1, 148)]
    public class PcfGridControl : Control, IPcfGridControl
    {
        private readonly string name;
        private readonly IPageFactory pageFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator root;
        private readonly ILocator rowsContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfGridControl"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public PcfGridControl(IPage page, string name, IPageFactory pageFactory, IControl parent = null, ILogger<PcfGridControl> logger = null)
            : base(page, parent)
        {
            this.name = name;
            this.pageFactory = pageFactory;
            this.logger = logger;

            this.root = this.Container.Locator($"div[data-lp-id*='MscrmControls.Grid.PCFGridControl|{this.name}']");
            this.rowsContainer = this.Container.Locator("div.ag-center-cols-viewport");
        }

        /// <inheritdoc/>
        protected override ILocator Root => this.root;

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.Page.WaitForAppIdleAsync();

            var row = this.GetRow(index);
            if (!await row.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {this.name}");
            }

            await row.DblClickAsync();

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        private ILocator GetRow(int index)
        {
            return this.rowsContainer.Locator($"div[role='row'][row-index='{index}']");
        }
    }
}