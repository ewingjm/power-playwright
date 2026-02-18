namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IGridControl"/>.
    /// </summary>
    [PcfControlStrategy(1, 11, 531)]
    public class GridControl : PcfControlInternal, IGridControl
    {
        private readonly IPageFactory pageFactory;
        private readonly IControlFactory controlFactory;
        private readonly ILogger<GridControl> logger;

        private readonly ILocator grid;
        private readonly ILocator visibleRows;
        private readonly ILocator visibleHeaders;
        private readonly ILocator alerts;
        private readonly ILocator rowsContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="name">The name given to the control.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        /// <param name="logger">The logger.</param>
        public GridControl(IAppPage appPage, string name, IEnvironmentInfoProvider infoProvider, IPageFactory pageFactory, IControlFactory controlFactory, IControl parent = null, ILogger<GridControl> logger = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.pageFactory = pageFactory;
            this.controlFactory = controlFactory;
            this.logger = logger;

            this.grid = this.Container.GetByRole(AriaRole.Grid);
            this.visibleRows = this.grid.GetByRole(AriaRole.Row);
            this.visibleHeaders = this.visibleRows.Locator("[role='columnheader']:not([aria-colindex='1'])").Filter(new LocatorFilterOptions { HasNot = this.Page.GetByRole(AriaRole.Img, new PageGetByRoleOptions { Name = "Navigate", Exact = true }) });
            this.rowsContainer = this.Container.Locator("[wj-part='root']");
            this.alerts = this.Container.GetByRole(AriaRole.Alert);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetColumnNamesAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var countToRemove = (this.Parent is IGridControl) ? 2 : 1;
            var columnCount = int.Parse(await this.grid.GetAttributeAsync("aria-colcount")) - countToRemove;

            var capturedColumns = new List<string>();

            await this.ScrollHorizontalToStartAsync();

            while (true)
            {
                var visibleColumns = await this.visibleHeaders.AllInnerTextsAsync();

                capturedColumns.AddRange(visibleColumns.Except(capturedColumns));
                if (capturedColumns.Count < columnCount)
                {
                    await this.ScrollHorizontalAsync((await this.visibleHeaders.Last.BoundingBoxAsync()).X);
                    continue;
                }

                break;
            }

            await this.ScrollHorizontalToStartAsync();

            return capturedColumns;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetEditableColumnsAsync(int rowIndex)
        {
            await this.ScrollHorizontalToStartAsync();

            var allColumns = await this.GetColumnNamesAsync();
            var editableColumns = new List<string>();

            var existingToggleState = await this.GetSelectedStateAsync(rowIndex);
            var columnNameByColIndex = Enumerable.Range(2, allColumns.Count()).ToDictionary(i => i, i => allColumns.ElementAt(i - 2));

            var row = this.GetRow(rowIndex);
            await row
                .GetByRole(AriaRole.Gridcell)
                .Filter(new LocatorFilterOptions { HasNot = this.Page.GetByRole(AriaRole.Checkbox) })
                .First
                .ClickAndWaitForAppIdleAsync();

            await this.Page.Keyboard.PressAsync("Escape");

            int lastColIndex = 0;
            while (lastColIndex <= allColumns.Count())
            {
                var handle = await this.Page.EvaluateHandleAsync("() => document.activeElement");
                var focusedCell = handle.AsElement();

                lastColIndex = int.Parse(await focusedCell.GetAttributeAsync("aria-colindex"));

                var isReadOnly = bool.TryParse(await focusedCell.GetAttributeAsync("aria-readonly"), out bool result) && result;
                if (!isReadOnly)
                {
                    editableColumns.Add(columnNameByColIndex[lastColIndex]);
                }

                await this.Page.Keyboard.PressAsync("ArrowRight");
            }

            await this.ScrollHorizontalToStartAsync();
            await this.ToggleSelectRowAsync(rowIndex, existingToggleState);

            return editableColumns;
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
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.Page.WaitForAppIdleAsync();

            var row = this.GetRow(index);
            if (!await row.GetByRole(AriaRole.Gridcell).First.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {this.Name}");
            }

            await row.GetByRole(AriaRole.Gridcell).Nth(0).ClickAsync();
            await row.GetByRole(AriaRole.Gridcell).Nth(0).DblClickAsync();

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task ToggleSelectRowAsync(int rowIndex, bool select = true)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.ScrollHorizontalToStartAsync();

            var row = this.GetRow(rowIndex);
            if (!await row.GetByRole(AriaRole.Gridcell).First.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{rowIndex}' is out of range for grid.");
            }

            var checkBoxCell = row.Locator("[aria-colindex='1']");

            var toggleCheckBox = checkBoxCell.GetByRole(AriaRole.Checkbox);
            if (select == await toggleCheckBox.IsCheckedAsync())
            {
                return;
            }

            await checkBoxCell.ClickAsync();
            await this.Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateRowAsync(int rowIndex, IDictionary<string, string> values)
        {
            var row = this.visibleRows.Filter(new LocatorFilterOptions { HasNot = this.Page.Locator("[aria-label='Header']"), Has = this.Page.GetByRole(AriaRole.Gridcell) }).Nth(rowIndex);
            var columnsToProcess = values.Keys.ToList();

            while (columnsToProcess.Count != 0)
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

                    var input = cell.GetByRole(AriaRole.Textbox).Or(cell.GetByRole(AriaRole.Combobox));
                    await input.FillAsync(values[column]);
                    await this.Page.Keyboard.PressAsync("Tab");
                    await this.Page.WaitForAppIdleAsync();

                    columnsToProcess.Remove(column);
                }

                if (columnsToProcess.Count != 0)
                {
                    await this.ScrollHorizontalAsync((await this.visibleHeaders.Last.BoundingBoxAsync()).X);
                }
            }
        }

        /// <inheritdoc/>
        public async Task ToggleSelectAllRowsAsync(bool select = true)
        {
            await this.Page.WaitForAppIdleAsync();

            var totalRowCount = await this.GetRows().CountAsync();
            if (totalRowCount == 0)
            {
                this.logger?.LogInformation("There are no rows in the grid to select.");
                return;
            }

            var rows = await this.GetRowsByToggledStateAsync(select);
            if (await rows.CountAsync() == totalRowCount)
            {
                this.logger?.LogInformation("All rows are already in the expected state.");
                return;
            }

            var btnSelectAll = this.Container.Locator("button[title='Select All']");
            do
            {
                await btnSelectAll.ClickAndWaitForAppIdleAsync();
            }
            while (await (await this.GetRowsByToggledStateAsync(select)).CountAsync() == totalRowCount);
        }

        /// <inheritdoc/>
        public async Task<int> GetSelectedRowCountAsync()
        {
            var rows = this.GetRows()
                .Filter(new LocatorFilterOptions { Has = this.Page.GetByRole(AriaRole.Checkbox, new PageGetByRoleOptions { Checked = true }) });

            var count = await rows.CountAsync();
            return count;
        }

        /// <inheritdoc/>
        public async Task<bool> GetSelectedStateAsync(int index)
        {
            await this.ScrollHorizontalToStartAsync();

            var row = this.GetRow(index);
            if (!await row.GetByRole(AriaRole.Gridcell).First.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {this.Name}");
            }

            return await row.GetByRole(AriaRole.Checkbox).IsCheckedAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataRow>> GetRowDataAsync()
        {
            var rows = await this.GetRows().AllAsync();

            var columnNames = (await this.GetColumnNamesAsync()).ToArray();
            var dataRows = Enumerable.Empty<DataRow>().ToList();

            foreach (var row in rows)
            {
                var rowData = await this.GetSingleRowDataAsync(row, columnNames);
                dataRows.Add(new DataRow(rowData));
                await this.ScrollHorizontalToStartAsync();
            }

            return dataRows;
        }

        /// <inheritdoc/>
        public async Task<IGridControl> ExpandNestedSubgridAsync(int rowIndex)
        {
            await this.Page.WaitForAppIdleAsync();

            var row = this.GetRow(rowIndex);
            if (!await row.GetByRole(AriaRole.Gridcell).First.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{rowIndex}' is out of range for subgrid {this.Name}");
            }

            var expandCell = row.GetByRole(AriaRole.Gridcell, new LocatorGetByRoleOptions { Name = "Show nested grid" });

            if (await expandCell.IsVisibleAsync())
            {
                await expandCell.Locator("span").First.ClickAsync();
                await this.Page.WaitForAppIdleAsync();
            }

            return this.controlFactory.CreateCachedInstance<IGridControl>(this.AppPage, this.Name, this);
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRowCountAsync()
        {
            return await this.GetRows().CountAsync();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ColumnSortSpec>> GetSortOrdersAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var sortOrders = new List<ColumnSortSpec>();
            await this.ExecuteColumnBasedActionAsync(async (columnName, header) =>
            {
                var ariaSort = await header.GetAttributeAsync("aria-sort");

                if (Enum.TryParse<ColumnSortOrder>(ariaSort, true, out var order))
                {
                    sortOrders.Add(new ColumnSortSpec(columnName, order));
                }
            });

            return sortOrders.AsReadOnly();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"//div[(starts-with(@data-lp-id, '{this.PcfControlAttribute.Name}|') or starts-with(@data-lp-id, '{this.GetControlId()}|')) and substring(@data-lp-id, string-length(@data-lp-id) - string-length('cc-grid') + 1) = 'cc-grid']").First;
        }

        private ILocator GetRows()
        {
            return this.grid.Locator($"[role='row'][aria-label='Data']");
        }

        private ILocator GetRow(int rowIndex)
        {
            return this.GetRows().Nth(rowIndex);
        }

        private async Task<ILocator> GetRowsByToggledStateAsync(bool select)
        {
            await this.ScrollHorizontalToStartAsync();
            return this.GetRows().Locator($"[aria-selected='{select}']");
        }

        private async Task ScrollHorizontalAsync(float deltaX)
        {
            if (!await this.CanScrollHorizontalAsync())
            {
                return;
            }

            await this.grid.HoverAsync();
            await this.Page.Mouse.WheelAsync(deltaX, 0);
            await this.Page.WaitForAppIdleAsync();
        }

        private async Task ScrollHorizontalToStartAsync()
        {
            var scrollPosition = await this.GetHorizontalScrollPositionAsync();
            if (scrollPosition > 0)
            {
                await this.ScrollHorizontalAsync(-scrollPosition);
            }
        }

        private async Task<bool> CanScrollHorizontalAsync()
        {
            return await this.rowsContainer.EvaluateAsync<bool>("el => el.scrollWidth > el.clientWidth");
        }

        private async Task<Dictionary<string, string>> GetSingleRowDataAsync(ILocator row, string[] columnNames)
        {
            var rowData = new Dictionary<string, string>();
            while (rowData.Count < columnNames.Length)
            {
                var visibleColumns = (await this.visibleHeaders.AllInnerTextsAsync())
                    .Select(s =>
                    {
                        var match = Regex.Match(s, @"\r?\n|\\n");
                        return match.Success ? s.Substring(0, match.Index) : s;
                    })
                    .ToList();

                var visibleCells = await row.Locator("[role='gridcell']:not(:has([role='checkbox'])):not(:has(input[type='checkbox']))").AllAsync();

                for (int i = 0; i < visibleColumns.Count; i++)
                {
                    var column = visibleColumns[i];
                    if (!rowData.ContainsKey(column) && i < visibleCells.Count)
                    {
                        rowData[column] = await visibleCells[i].InnerTextAsync();
                    }
                }

                if (rowData.Count < columnNames.Length)
                {
                    await this.ScrollHorizontalAsync((await this.visibleHeaders.Last.BoundingBoxAsync()).X);
                }
            }

            return rowData;
        }

        private async Task<int> GetHorizontalScrollPositionAsync()
        {
            if (!await this.rowsContainer.IsVisibleAsync())
            {
                return 0;
            }

            return await this.rowsContainer.EvaluateAsync<int>("el => el.scrollLeft");
        }

        private async Task ExecuteColumnBasedActionAsync(Func<string, ILocator, Task> action)
        {
            await this.ScrollHorizontalToStartAsync();
            var allColumns = await this.GetColumnNamesAsync();
            var processedColumns = new HashSet<string>();

            while (true)
            {
                var visibleColumns = await this.Container.Locator("[role='columnheader']:not([aria-colindex='1'])").AllAsync();

                foreach (var column in visibleColumns)
                {
                    var columnName = await column.InnerTextAsync();
                    if (processedColumns.Contains(columnName))
                    {
                        continue;
                    }

                    await action(columnName, column);
                    processedColumns.Add(columnName);
                }

                if (processedColumns.Count < allColumns.Count())
                {
                    await this.ScrollHorizontalAsync(50);
                    await this.Page.WaitForAppIdleAsync();
                    continue;
                }

                break;
            }

            await this.ScrollHorizontalToStartAsync();
        }
    }
}