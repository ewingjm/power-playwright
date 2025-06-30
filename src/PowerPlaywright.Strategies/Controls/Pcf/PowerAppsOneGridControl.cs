namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    /// A control strategy for the <see cref="IPowerAppsOneGrid"/>.
    /// </summary>
    [PcfControlStrategy(1, 0, 208)]
    public class PowerAppsOneGridControl : PcfControlInternal, IPowerAppsOneGrid
    {
        private readonly IPageFactory pageFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator rowsContainer;
        private readonly ILocator columnHeaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerAppsOneGridControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="infoProvider"> The info provider.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public PowerAppsOneGridControl(IAppPage appPage, string name, IEnvironmentInfoProvider infoProvider, IPageFactory pageFactory, IControl parent = null, ILogger<PcfGridControl> logger = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.pageFactory = pageFactory;
            this.logger = logger;

            this.rowsContainer = this.Container.Locator("div.ag-center-cols-viewport");
            this.columnHeaders = this.Container.GetByRole(AriaRole.Columnheader);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetColumnNamesAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var columnHeaders = await this.columnHeaders.AllAsync();

            return await Task.WhenAll(columnHeaders.Skip(1).Select(c => c.Locator("label").InnerTextAsync()));
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.Page.WaitForAppIdleAsync();

            await rowsContainer.WaitForAsync();

            var row = this.GetRow(index);
            if (!await row.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {Name}");
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