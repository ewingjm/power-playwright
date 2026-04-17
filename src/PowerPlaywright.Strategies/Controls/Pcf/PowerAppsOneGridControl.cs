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
            this.gridHeaderContainer = this.Container.GetByRole(AriaRole.Rowgroup).Filter(new LocatorFilterOptions { Has = this.Page.GetByRole(AriaRole.Columnheader) });
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetColumnNamesAsync()
        {
            await this.Page.WaitForAppIdleAsync();
            await this.ScrollHorizontalToStartAsync();

            var columnNames = new List<string>();

            await this.ExecuteColumnBasedActionAsync((column, locator) =>
            {
                columnNames.Add(column);
            });

            return columnNames;
        }

        /// <inheritdoc/>
        public async Task<int> GetTotalRowCountAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var pattern = @"Rows:\s+(\d+)";
            var statusTextContainers = await this.Container.Locator("span[class*='statusTextContainer-']").AllTextContentsAsync();
            var match = statusTextContainers.Select(st => Regex.Match(st, pattern, RegexOptions.CultureInvariant))
                .FirstOrDefault(m => m.Success);

            return match == null
                ? throw new PowerPlaywrightException($"Unable to get total row count from status text: {string.Join(" ", statusTextContainers)}.")
                : int.Parse(match.Groups[1].Value);
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

            await row.GetByRole(AriaRole.Gridcell).Nth(0).DispatchEventAsync("dblclick");
            await this.Page.WaitForAppIdleAsync();

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
            await this.Page.WaitForAppIdleAsync();

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
            var rowCount = await this.GetRows().CountAsync();

            var dataRows = Enumerable.Range(0, rowCount).Select(i => new Dictionary<string, string>()).ToArray();
            var rows = await this.rowsContainer.Locator("div[role='row']:not(:has([role='columnheader']))").AllAsync();

            await this.ExecuteColumnBasedActionAsync(async (columnName, header) =>
            {
                var colIndex = await header.GetAttributeAsync(Attributes.AriaColIndex);

                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    var cell = this.GetRow(rowIndex).Locator($"[role='gridcell'][aria-colindex='{colIndex}']");
                    dataRows[rowIndex][columnName] = await cell.InnerTextAsync();
                }
            });

            return dataRows.Select(d => new DataRow(d));
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
            await this.Page.WaitForAppIdleAsync();

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

        private async Task ExecuteColumnBasedActionAsync(Action<string, ILocator> action)
        {
            await this.ExecuteColumnBasedActionAsync((column, locator) =>
            {
                action(column, locator);
                return Task.CompletedTask;
            });
        }

        private async Task ExecuteColumnBasedActionAsync(Func<string, ILocator, Task> action)
        {
            await TimeoutGuard.ExecuteWithTimeoutAsync(async () =>
            {
                await this.ScrollHorizontalToStartAsync();

                var processedColumns = new HashSet<string>();
                var columnCount = int.Parse(await this.treeGrid.GetAttributeAsync(Attributes.AriaColCount)) - 1;

                await this.Container.Locator("[role='columnheader'][aria-colindex='2']").FocusAsync();
                while (processedColumns.Count < columnCount)
                {
                    var column = this.Container.Locator("[role='columnheader']:focus");

                    if (await column.Filter(new LocatorFilterOptions { Has = this.Page.GetByRole(AriaRole.Img, new PageGetByRoleOptions { Name = "Navigate", Exact = true }) }).First.IsVisibleAsync())
                    {
                        break;
                    }

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

                    if (processedColumns.Count < columnCount)
                    {
                        await this.Page.Keyboard.PressAsync("ArrowRight");
                        await this.Page.WaitForAppIdleAsync();
                        continue;
                    }

                    break;
                }

                await this.ScrollHorizontalToStartAsync();
            });
        }
    }
}