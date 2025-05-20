namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// OptionSet control class.
    /// </summary>
    public interface IOptionSet : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single select optionset.
        /// </summary>
        /// <param name="optionValue">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string optionValue);

        /// <summary>
        /// Gets the value of the single select optionset.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}