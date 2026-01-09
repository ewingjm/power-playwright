namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Read-only grid control class.
    /// </summary>
    public interface IReadOnlyGrid : IPcfControl
    {
        /// <summary>
        /// Opens the record at the provided zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the record.</param>
        /// <returns>The record.</returns>
        Task<IEntityRecordPage> OpenRecordAsync(int index);

        /// <summary>
        /// Gets the column names on the view.
        /// </summary>
        /// <returns>The column names.</returns>
        Task<IEnumerable<string>> GetColumnNamesAsync();

        /// <summary>
        /// Gets the total number of rows across all pages.
        /// </summary>
        /// <returns>The total number of rows.</returns>
        Task<int> GetTotalRowCountAsync();

        /// <summary>
        /// Toggles all rows within the grid to the specified state.
        /// </summary>
        /// <param name="select">Desired state of the checkbox.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task ToggleSelectAllRowsAsync(bool select = true);

        /// <summary>
        /// Gets the number of selected rows in the grid.
        /// </summary>
        /// <returns>The total number of rows.</returns>
        Task<int> GetSelectedRowCountAsync();

        /// <summary>
        /// Gets the row data from the grid as a collection of dictionaries.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<IDictionary<string, string>>> GetRowDataAsync();

        /// <summary>
        /// Toggles the selection state of a specific row.
        /// </summary>
        /// <param name="index">The zero-based index of the row.</param>
        /// <param name="select">Desired state of the checkbox.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task ToggleSelectRowAsync(int index, bool select = true);

        /// <summary>
        /// Gets the current sort orders applied to the grid.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IReadOnlyList<ColumnSortSpec>> GetSortOrdersAsync();

        /// <summary>
        /// Searches in a grid.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SearchAsync(string searchTerm);
    }
}