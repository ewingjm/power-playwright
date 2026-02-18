namespace PowerPlaywright.Framework.Controls.Pcf
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the MscrmControls.Grid.GridControl control.
    /// </summary>
    [PcfControl("MscrmControls.Grid.GridControl")]
    public interface IGridControl : IPcfControl, IReadOnlyGrid
    {
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
        /// Gets a nested subgrid for the specified row index.
        /// </summary>
        /// <param name="rowIndex">The zero-based index of the record.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IGridControl> ExpandNestedSubgridAsync(int rowIndex);
    }
}