namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A control strategy for the <see cref="IGridControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 11, 531)]
    public class GridControl : PcfControlInternal, IGridControl
    {
        private readonly IPageFactory pageFactory;
        private readonly ILocator grid;
        private readonly ILocator visibleRows;
        private readonly ILocator visibleHeaders;
        private readonly ILocator alerts;
        private ILocator scrollableContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="parent">The parent control.</param>
        public GridControl(IAppPage appPage, string name, IEnvironmentInfoProvider infoProvider, IPageFactory pageFactory, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.pageFactory = pageFactory;
            this.grid = this.Container.GetByRole(AriaRole.Grid);
            this.visibleRows = this.grid.GetByRole(AriaRole.Row);
            this.visibleHeaders = this.visibleRows.Locator("[role='columnheader']:not([aria-colindex='1'])").Filter(new LocatorFilterOptions { HasNot = Page.GetByRole(AriaRole.Img, new PageGetByRoleOptions { Name = "Navigate", Exact = true }) });
            this.scrollableContainer = this.Container.Locator("[wj-part='root']");
            this.alerts = this.Container.GetByRole(AriaRole.Alert);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetColumnNamesAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var columnCount = int.Parse(await this.grid.GetAttributeAsync("aria-colcount")) - 1;
            var capturedColumns = new List<string>();
            var viewPortWidth = await this.GetRowsViewPortWidthAsync();

            await this.ScrollHorizontalToStartAsync(this.scrollableContainer);

            while (true)
            {
                var visibleColumns = await this.visibleHeaders.AllInnerTextsAsync();

                capturedColumns.AddRange(visibleColumns.Except(capturedColumns));
                if (capturedColumns.Count < columnCount)
                {
                    await this.ScrollHorizontalAsync(viewPortWidth / 2);
                    continue;
                }

                break;
            }

            await this.ScrollHorizontalToStartAsync(this.scrollableContainer);

            return capturedColumns;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetEditableColumnsAsync(int rowIndex)
        {
            var allColumns = await this.GetColumnNamesAsync();
            var columnsByEditability = allColumns.ToDictionary(col => col, col => true);

            var existingToggleState = await this.GetToggledState(rowIndex + 1);
            var columnNameByColIndex = Enumerable.Range(2, allColumns.Count()).ToDictionary(i => i, i => allColumns.ElementAt(i - 2));
            var rowsViewportWidth = await this.GetRowsViewPortWidthAsync();
            var row = this.visibleRows.Nth(rowIndex + 1);

            while (true)
            {
                var cells = row.GetByRole(AriaRole.Gridcell);
                await this.EnsureReadOnlyIconsAreVisibleAsync(cells);  // Trigger the aria-readonly attributes

                var visibleCells = await cells.Filter(new LocatorFilterOptions { HasNot = this.Page.GetByRole(AriaRole.Checkbox) }).AllAsync();

                int lastColIndex = 0;
                foreach (var visibleCell in visibleCells)
                {
                    var colIndex = int.Parse(await visibleCell.GetAttributeAsync("aria-colindex"));
                    if (colIndex == 1)
                    {
                        continue;
                    }

                    lastColIndex = colIndex;
                    var columnName = columnNameByColIndex[lastColIndex];

                    if (!columnsByEditability[columnName])
                    {
                        continue;
                    }

                    var isReadOnly = bool.TryParse(await visibleCell.GetAttributeAsync("aria-readonly"), out bool result) && result;

                    if (isReadOnly)
                    {
                        columnsByEditability[columnName] = false;
                    }
                }

                if (lastColIndex < allColumns.Count())
                {
                    await this.ScrollHorizontalAsync(rowsViewportWidth / 2);
                    continue;
                }

                break;
            }

            await this.ScrollHorizontalToStartAsync(this.scrollableContainer);
            await this.ToggleSelectRowAsync(rowIndex, existingToggleState);

            return columnsByEditability.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
        }

        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> GetErrorNotificationsAsync()
        {
            var pattern = @"Error at row (?<row>\d+), (?<field>[\w ]+): (?<message>.+)";

            var errors = (await this.alerts.AllTextContentsAsync())
                .Select(e => Regex.Match(e, pattern))
                .Where(m => m.Success)
                .ToDictionary(
                    m => m.Groups["field"].Value,
                    m => m.Groups["message"].Value);

            return errors;
        }

        /// <inheritdoc/>
        public async Task<bool> GetToggledState(int rowIndex)
        {
            var row = this.visibleRows.Nth(rowIndex);

            return await row.GetByRole(AriaRole.Checkbox).IsCheckedAsync();
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.Page.WaitForAppIdleAsync();

            var row = this.GetRow(index);
            if (await row.CountAsync() != 1)
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {this.Name}");
            }

            await row.GetByRole(AriaRole.Gridcell).Nth(0).ClickAsync();
            await row.GetByRole(AriaRole.Gridcell).Nth(0).DblClickAsync();

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task ScrollHorizontalAsync(float deltaX)
        {
            var cell = this.scrollableContainer.GetByRole(AriaRole.Row).GetByRole(AriaRole.Gridcell).First;

            if (await cell.IsVisibleAsync())
            {
                this.scrollableContainer = cell;
            }

            await this.scrollableContainer.HoverAsync();
            await this.scrollableContainer.Page.Mouse.WheelAsync(deltaX, 0);
            await this.scrollableContainer.Page.WaitForAppIdleAsync();

            // TODO: Return position
        }

        /// <inheritdoc/>
        public async Task ToggleSelectRowAsync(int rowIndex, bool select = true)
        {
            await this.Page.WaitForAppIdleAsync();

            var rows = this.grid.GetByRole(AriaRole.Row);
            var cell = rows.GetByRole(AriaRole.Gridcell).Filter(new LocatorFilterOptions { Has = this.Page.GetByRole(AriaRole.Checkbox) }).Nth(rowIndex);

            var toggleCheckBox = cell.GetByRole(AriaRole.Checkbox);
            if (select == await toggleCheckBox.IsCheckedAsync())
            {
                return;
            }

            await cell.ClickAsync();
            await this.Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateRowAsync(int rowIndex, IDictionary<string, string> values)
        {
            var viewPortWidth = await this.GetRowsViewPortWidthAsync();
            var row = this.visibleRows.Filter(new LocatorFilterOptions { HasNot = this.Page.Locator("[aria-label='Header']"), Has = this.Page.GetByRole(AriaRole.Gridcell) }).Nth(rowIndex);
            var columnsToProcess = values.Keys.ToList();

            while (columnsToProcess.Any())
            {
                var visibleColumns = (await this.visibleHeaders.AllInnerTextsAsync()).ToList();
                var visibleColumnsToProcess = visibleColumns.Where(columnsToProcess.Contains);

                foreach (var column in visibleColumnsToProcess)
                {
                    var cell = row.GetByRole(AriaRole.Gridcell)
                        .Filter(new LocatorFilterOptions { HasNot = this.Page.GetByRole(AriaRole.Checkbox) })
                        .Nth(visibleColumns.IndexOf(column));

                    await cell.ClickAsync();
                    await this.Page.WaitForAppIdleAsync();

                    var input = cell.GetByRole(AriaRole.Textbox);
                    await input.FillAsync(values[column]);
                    await this.Page.Keyboard.PressAsync("Tab");
                    await this.Page.WaitForAppIdleAsync();

                    columnsToProcess.Remove(column);
                }

                if (columnsToProcess.Any())
                {
                    await this.ScrollHorizontalAsync(viewPortWidth / 2);
                }
            }
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return this
                .Page
                .Locator($"//div[@data-id=\"DataSetHostContainer\"][.//div[starts-with(@data-lp-id, '{this.PcfControlAttribute}|') or starts-with(@data-lp-id, '{this.GetControlId()}|')]]");
        }

        private async Task EnsureReadOnlyIconsAreVisibleAsync(ILocator cells, int maxAttempts = 5)
        {
            var cell = cells.Nth(0);
            var readOnlyIcons = cells.Locator("span[editable-grid-icon='true']");

            for (int i = 0; i < maxAttempts; i++)
            {
                await cell.ClickAsync();
                await this.Page.WaitForAppIdleAsync();

                if (await readOnlyIcons.CountAsync() > 0)
                {
                    return;
                }
            }

            throw new TimeoutException("The read-only icons did not appear.");
        }

        private async Task<float> GetRowsViewPortWidthAsync()
        {
            var rowsBoundingBox = await this.Container.BoundingBoxAsync();

            return rowsBoundingBox.Width;
        }

        private async Task ScrollHorizontalToStartAsync(ILocator scrollableContainer)
        {
            if (await this.scrollableContainer.EvaluateAsync<bool>("el => el.scrollWidth <= el.clientWidth"))
            {
                return;
            }

            var scrollLeft = await this.scrollableContainer.EvaluateAsync<int>("el => el.scrollLeft");
            var cell = scrollableContainer.GetByRole(AriaRole.Row).GetByRole(AriaRole.Gridcell).First;

            if (await cell.CountAsync() == 1)
            {
                scrollableContainer = cell;
            }
            else
            {
                var noDataAvailable = this.Container.GetByTitle("No data available.");

                if (await noDataAvailable.IsVisibleAsync())
                {
                    scrollableContainer = this.Container.GetByRole(AriaRole.Grid);
                }
            }

            await scrollableContainer.HoverAsync();
            await scrollableContainer.Page.Mouse.WheelAsync(-scrollLeft, 0);
            await scrollableContainer.Page.WaitForAppIdleAsync();
        }

        private ILocator GetRow(int rowIndex)
        {
            return this.scrollableContainer.Locator($"[role='row'][aria-label='Data']").Nth(rowIndex);
        }
    }
}