namespace PowerPlaywright.Framework.Controls.Pcf
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// An interface for the Microsoft.PowerApps.PowerAppsOneGrid control.
    /// </summary>
    [PcfControl("Microsoft.PowerApps.PowerAppsOneGrid")]
    public interface IPowerAppsOneGrid : IReadOnlyGrid
    {
        /// <summary>
        /// Gets a nested subgrid for the specified row index.
        /// </summary>
        /// <param name="rowIndex">The zero-based index of the record.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IPowerAppsOneGrid> ExpandNestedSubgridAsync(int rowIndex);
    }
}