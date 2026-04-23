namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Whole number timezone control class.
    /// </summary>
    public interface IWholeNumberTimezone : IPcfControl
    {
        /// <summary>
        /// Sets the value of the timezone control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the timezone control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();

        /// <summary>
        /// Gets all available options of the timezone control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<string>> GetAllOptionsAsync();
    }
}
