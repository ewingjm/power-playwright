namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Picklist (Choices) control class.
    /// </summary>
    public interface IUpdMSPicklist : IPcfControl
    {
        /// <summary>
        /// Sets the value of the multi select picklist.
        /// </summary>
        /// <param name="optionValues">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(List<string> optionValues);

        /// <summary>
        /// Gets the value of the multi select picklist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Dictionary<int, string>> GetValueAsync();

        /// <summary>
        /// Selects all the values of the multi select picklist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SelectAllAsync();
    }
}