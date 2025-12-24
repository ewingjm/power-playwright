namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Pages;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// GridControl control class.
    /// </summary>
    public interface IEditableGridControl : IPcfControl
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
        Task<bool> GetToggledState(int rowIndex);

        /// <summary>
        /// Toggles all rows within the grid to the specified state.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="select">Desired state of the checkbox.</param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task ToggleSelectRowAsync(int rowIndex, bool select = true);

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
        /// Scroll horizontal.
        /// </summary>
        /// <param name="deltaX"></param>
        /// <returns>A <see cref="Task"/> representing an asynchronous operation.</returns>
        Task ScrollHorizontalAsync(float deltaX);

        /// <summary>
        /// Opens the record at the provided zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index of the record.</param>
        /// <returns>The record.</returns>
        Task<IEntityRecordPage> OpenRecordAsync(int index);
    }
}