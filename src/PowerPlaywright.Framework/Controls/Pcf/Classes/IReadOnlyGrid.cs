namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        /// Gets the row data from the currently visible page.
        /// </summary>
        /// <returns>The rows with column name to cell value mappings.</returns>
        Task<IEnumerable<IDictionary<string, string>>> GetRowsAsync();
    }
}