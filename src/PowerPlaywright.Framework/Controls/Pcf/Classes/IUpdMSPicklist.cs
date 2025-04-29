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
        Task SetValueAsync(int[] optionValues);

        /// <summary>
        /// Gets the value of the multi select picklist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int?[]> GetValueAsync();
    }
}