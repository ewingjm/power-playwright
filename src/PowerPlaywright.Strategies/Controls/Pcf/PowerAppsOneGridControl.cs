namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
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
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IPowerAppsOneGrid"/>.
    /// </summary>
    [PcfControlStrategy(1, 0, 208)]
    public class PowerAppsOneGridControl : PcfControlInternal, IPowerAppsOneGrid
    {
        private readonly IPageFactory pageFactory;
        private readonly ILogger<PcfGridControl> logger;

        private readonly ILocator treeGrid;
        private readonly ILocator rowsContainer;
        private readonly ILocator columnHeaders;
        private readonly ILocator gridHeaderContainer;

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

            this.treeGrid = this.Container.GetByRole(AriaRole.Treegrid);
            this.rowsContainer = this.Container.Locator("div.ag-center-cols-viewport");
            this.columnHeaders = this.Container.Locator("[role='columnheader']:not([aria-colindex='1'])").Filter(new LocatorFilterOptions { HasNot = this.Page.GetByRole(AriaRole.Img, new PageGetByRoleOptions { Name = "Navigate", Exact = true }) });
            this.gridHeaderContainer = this.Container.GetByRole(AriaRole.Rowgroup).Filter(new LocatorFilterOptions { Has = this.Page.GetByRole(AriaRole.Columnheader) });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetColumnNamesAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var columnCount = int.Parse(await this.treeGrid.GetAttributeAsync(Attributes.AriaColCount)) - 1;
            var rowsBoundingBox = await this.rowsContainer.BoundingBoxAsync();
            var capturedColumns = new List<string>();

            await this.Container.Locator("[role='columnheader'][aria-colindex='1']").FocusAsync();

            for (int i = 0; i < columnCount; i++)
            {
                var visibleColumns = (await this.columnHeaders.AllInnerTextsAsync())
                    .Select(s =>
                    {
                        var match = Regex.Match(s, @"\r?\n|\\n");
                        return match.Success ? s.Substring(0, match.Index) : s;
                    })
                    .ToList();

                capturedColumns.AddRange(visibleColumns.Except(capturedColumns));

                await this.Page.Keyboard.PressAsync("ArrowRight");
            }

            await this.ScrollHorizontalToStartAsync();

            return capturedColumns;
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRowCountAsync()
        {
            var pattern = @"Rows:\s+(\d+)";
            var statusTextContainers = await this.Container.Locator("span[class*='statusTextContainer-']").AllTextContentsAsync();
            var match = statusTextContainers.Select(st => Regex.Match(st, pattern, RegexOptions.CultureInvariant))
                .FirstOrDefault(m => m.Success);

            if (match == null)
            {
                throw new PowerPlaywrightException($"Unable to get total row count from status text: {string.Join(" ", statusTextContainers)}.");
            }

            return int.Parse(match.Groups[1].Value);
        }

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> OpenRecordAsync(int index)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.rowsContainer.WaitForAsync();

            var row = this.GetRow(index);
            if (!await row.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for subgrid {this.Name}");
            }

            await row.GetByRole(AriaRole.Gridcell).Nth(1).DblClickAsync(new LocatorDblClickOptions { Position = new Position { X = 0, Y = 0 } });

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }

        /// <inheritdoc/>
        public async Task ToggleSelectAllRowsAsync(bool select = true)
        {
            await this.Page.WaitForAppIdleAsync();

            var totalRowCount = await this.GetTotalRowCountAsync();
            if (totalRowCount == 0)
            {
                this.logger?.LogInformation("There are no rows in the grid to select.");
                return;
            }

            var toggleCheckBox = this.gridHeaderContainer.GetByRole(AriaRole.Checkbox, new LocatorGetByRoleOptions { Name = "Toggle selection of all rows" });
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

            await toggleCheckBox.Locator("..").ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<int> GetSelectedRowCountAsync()
        {
            var pattern = @"Selected:\s*(\d+)";
            var statusTextContainers = await this.Container.Locator("span[class*='statusTextContainer-']").AllTextContentsAsync();

            return statusTextContainers.Select(st => Regex.Match(st, pattern, RegexOptions.CultureInvariant))
                .Where(m => m.Success)
                .Select(m => int.Parse(m.Groups[1].Value))
                .FirstOrDefault();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DataRow>> GetRowDataAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var rows = await this.rowsContainer.Locator("div[role='row']:not(:has([role='columnheader']))").AllAsync();
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
        public async Task ToggleSelectRowAsync(int index, bool select = true)
        {
            await this.Page.WaitForAppIdleAsync();

            var row = this.GetRow(index);
            if (!await row.IsVisibleAsync())
            {
                throw new IndexOutOfRangeException($"The provided index '{index}' is out of range for grid.");
            }

            var checkboxCell = row.Locator("[role='gridcell']").First;
            var checkbox = checkboxCell.GetByRole(AriaRole.Checkbox);

            // Check if it exists first
            if (await checkbox.CountAsync() == 0)
            {
                throw new PowerPlaywrightException($"Unable to find checkbox for row at index '{index}'.");
            }

            var currentState = await checkbox.IsCheckedAsync();

            if (currentState != select)
            {
                await checkboxCell.ClickAndWaitForAppIdleAsync();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> GetSelectedStateAsync(int index)
        {
            await this.ScrollHorizontalToStartAsync();

            var row = this.GetRow(index);

            return await row.GetByRole(AriaRole.Checkbox).IsCheckedAsync();
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

        private ILocator GetRows()
        {
            return this.rowsContainer.Locator($"div[role='row']");
        }

        private ILocator GetRow(int index)
        {
            return this.GetRows().Nth(index);
        }

        private async Task<bool> CanScrollHorizontalAsync()
        {
            return await this.rowsContainer.EvaluateAsync<bool>("el => el.scrollWidth > el.clientWidth");
        }

        private async Task ScrollHorizontalAsync(float deltaX)
        {
            if (!await this.CanScrollHorizontalAsync())
            {
                return;
            }

            await this.rowsContainer.HoverAsync();
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

        private async Task<int> GetHorizontalScrollPositionAsync()
        {
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
                    var sanitisedColumnName = Regex.Match(columnName, @"\r?\n|\\n").Success
                        ? Regex.Replace(columnName, @"\r?\n|\\n|\p{C}", string.Empty)
                        : columnName;

                    if (processedColumns.Contains(sanitisedColumnName))
                    {
                        continue;
                    }

                    await action(sanitisedColumnName, column);
                    processedColumns.Add(sanitisedColumnName);
                }

                if (processedColumns.Count < allColumns.Count())
                {
                    await this.ScrollHorizontalAsync((await this.columnHeaders.Last.BoundingBoxAsync()).X / 2);
                    await this.Page.WaitForAppIdleAsync();
                    continue;
                }

                break;
            }

            await this.ScrollHorizontalToStartAsync();
        }

        private async Task<Dictionary<string, string>> GetSingleRowDataAsync(ILocator row, string[] columnNames)
        {
            var rowData = new Dictionary<string, string>();

            while (rowData.Count < columnNames.Length)
            {
                var visibleColumns = (await this.columnHeaders.AllInnerTextsAsync())
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
                    await this.ScrollHorizontalAsync((await this.columnHeaders.Last.BoundingBoxAsync()).X / 2);
                }
            }

            return rowData;
        }
    }
}