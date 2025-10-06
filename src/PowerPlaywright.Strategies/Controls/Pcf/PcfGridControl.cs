namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
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
    public class PcfGridControl : PcfControlInternal, IPcfGridControl
    {
        private readonly IPageFactory pageFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator treeGrid;
        private readonly ILocator rowsContainer;
        private readonly ILocator columnHeaders;
        private readonly ILocator gridHeaderContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PcfGridControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public PcfGridControl(IAppPage appPage, string name, IEnvironmentInfoProvider infoProvider, IPageFactory pageFactory, IControl parent = null, ILogger<PcfGridControl> logger = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.pageFactory = pageFactory;
            this.logger = logger;

            this.treeGrid = this.Container.GetByRole(AriaRole.Treegrid);
            this.rowsContainer = this.Container.Locator("div.ag-center-cols-viewport");
            this.columnHeaders = this.Container.Locator("[role='columnheader']:not([aria-colindex='1'])");
            this.gridHeaderContainer = this.Container.Locator("div[class='ag-header-container']");
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetColumnNamesAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var columnCount = int.Parse(await this.treeGrid.GetAttributeAsync(Attributes.AriaColCount)) - 1;
            var rowsBoundingBox = await this.rowsContainer.BoundingBoxAsync();
            var capturedColumns = new List<string>();
            while (true)
            {
                var visibleColumns = await this.columnHeaders.AllAsync();
                var visibleColumnLabels = await Task.WhenAll(visibleColumns.Select(c => c.Locator("label").InnerTextAsync()));

                capturedColumns.AddRange(visibleColumnLabels.Except(capturedColumns));
                if (capturedColumns.Count < columnCount)
                {
                    await this.rowsContainer.HoverAsync();
                    await this.Page.Mouse.WheelAsync(rowsBoundingBox.Width, 0);
                    await this.Page.WaitForAppIdleAsync();

                    continue;
                }

                break;
            }

            return capturedColumns;
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

            await row.GetByRole(AriaRole.Gridcell).Nth(1).DblClickAsync(new LocatorDblClickOptions { Position = new Position { X = 0, Y = 0 } });

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRowCountAsync()
        {
            var statusText = await this.Container.Locator("span[class*='statusContainer-'] div[role='status']").TextContentAsync();

            var match = Regex.Match(statusText, @"of\s+(\d+)");
            if (!match.Success)
            {
                throw new PowerPlaywrightException($"Unable to get total row count from status text: {statusText}.");
            }

            return int.Parse(match.Groups[1].Value);
        }

        /// <inheritdoc/>
        public async Task ToggleAllRowsAsync(bool select = true)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.rowsContainer.WaitForAsync();

            var totalRowCount = await this.GetTotalRowCountAsync();
            if (totalRowCount == 0)
            {
                this.logger?.LogInformation("There are no rows in the grid to select.");
                return;
            }

            var toggleCheckBox = this.gridHeaderContainer.Locator("div[class*='ms-Checkbox-checkbox'] i[class*='ms-Checkbox-checkmark']");
            if (toggleCheckBox == null)
            {
                throw new PowerPlaywrightException($"Unable to find the select all checkbox within the {this.Name} grid header.");
            }

            var currentState = await toggleCheckBox.IsCheckedAsync();
            if (currentState == select)
            {
                this.logger?.LogInformation("All rows are already in the expected state.");
                return;
            }

            await toggleCheckBox.ClickAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<bool>> GetRowSelectionStatesAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.rowsContainer.WaitForAsync();

            var rowCheckBoxes = this.rowsContainer.Locator("div[class*='ms-Checkbox-checkbox'] i[class*='ms-Checkbox-checkmark']");
            var totalCheckBoxCount = await rowCheckBoxes.CountAsync();
            var states = new List<bool>(totalCheckBoxCount);

            for (int i = 0; i < totalCheckBoxCount; i++)
            {
                states.Add(await rowCheckBoxes.Nth(i).IsCheckedAsync());
            }

            return states;
        }

        private ILocator GetRow(int index)
        {
            return this.rowsContainer.Locator($"div[role='row'][row-index='{index}']");
        }
    }
}