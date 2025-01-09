﻿namespace PowerPlaywright.Strategies.Controls.Pcf
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
    public class PcfGridControl : PcfControl, IPcfGridControl
    {
        private readonly IPageFactory pageFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator rowsContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfGridControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public PcfGridControl(IAppPage appPage, string name, IPageFactory pageFactory, IControl parent = null, ILogger<PcfGridControl> logger = null)
            : base(name, appPage, parent)
        {
            this.pageFactory = pageFactory;
            this.logger = logger;

            this.rowsContainer = this.Container.Locator("div.ag-center-cols-viewport");
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.Page.WaitForAppIdleAsync();

            var row = this.GetRow(index);
            if (!await row.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {this.Name}");
            }

            await row.DblClickAsync();

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc />
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.Grid.PCFGridControl|{this.Name}']");
        }

        private ILocator GetRow(int index)
        {
            return this.rowsContainer.Locator($"div[role='row'][row-index='{index}']");
        }
    }
}