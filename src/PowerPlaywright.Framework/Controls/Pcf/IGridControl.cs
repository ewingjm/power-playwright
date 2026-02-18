namespace PowerPlaywright.Framework.Controls.Pcf
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An interface for the MscrmControls.Grid.GridControl control.
    /// </summary>
    [PcfControl("MscrmControls.Grid.GridControl")]
    public interface IGridControl : IPcfControl
    {
        /// <summary>
        /// Gets the column names on the view.
        /// </summary>
        /// <returns>The column names.</returns>
        Task<IEnumerable<string>> GetColumnNamesAsync();

        /// <summary>
        /// Gets the toggled state for a given row.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task<bool> GetToggledStateAsync(int rowIndex);

        /// <summary>
        /// Toggles a row for the given row index to the specified state.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="select">Desired state of the checkbox.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task ToggleSelectRowAsync(int rowIndex, bool select = true);

        /// <summary>
        /// Toggles all rows within the grid to the specified state.
        /// </summary>
        /// <param name="select">Desired state of the checkbox.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task ToggleSelectAllRowsAsync(bool select = true);

        /// <summary>
        /// Gets which columns are editable for the provided (zero-based) row index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task<IEnumerable<string>> GetEditableColumnsAsync(int rowIndex);

        /// <summary>
        /// Updates a row.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="values">The values.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task UpdateRowAsync(int rowIndex, IDictionary<string, string> values);

        /// <summary>
        /// Gets the error notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task<Dictionary<string, string>> GetErrorNotificationsAsync();

        /// <summary>
        /// Opens the record at the provided zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the record.</param>
        /// <returns>The record.</returns>
        Task<IEntityRecordPage> OpenRecordAsync(int index);

        /// <summary>
        /// Gets the number of selected rows in the grid.
        /// </summary>
        /// <returns>The total number of rows.</returns>
        Task<int> GetSelectedRowCountAsync();

        /// <summary>
        /// Gets the row data from the grid as a collection of dictionaries.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<DataRow>> GetRowDataAsync();

        /// <summary>
        /// Searches in a grid.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SearchAsync(string searchTerm);

        /// <summary>
        /// Gets a nested subgrid for the specified row index.
        /// </summary>
        /// <param name="rowIndex">The zero-based index of the record.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IGridControl> ExpandNestedSubgridAsync(int rowIndex);

        /// <summary>
        /// Gets the total number of rows across all pages.
        /// </summary>
        /// <returns>The total number of rows.</returns>
        Task<int> GetTotalRowCountAsync();
    }
}