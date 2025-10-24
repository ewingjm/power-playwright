namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// Lookup control class.
    /// </summary>
    public interface ILookup : IPcfControl
    {
        /// <summary>
        /// Sets the value of the lookup.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the lookup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();

        /// <summary>
        /// Creates a new record for the lookup via quick create.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IQuickCreateForm> NewViaQuickCreateAsync();

        /// <summary>
        /// Gets the lookup item names within the results view.
        /// </summary>
        /// <param name="search">The search term.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<List<string>>> GetSearchResultsAsync(string search = "");
    }
}