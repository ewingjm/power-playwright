namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Choices control class.
    /// </summary>
    public interface IChoices : IPcfControl
    {
        /// <summary>
        /// Sets the value of the choices control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(IEnumerable<string> value);

        /// <summary>
        /// Gets the value of the choices control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEnumerable<string>> GetValueAsync();

        /// <summary>
        /// Selects all the values of the choices control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SelectAllAsync();
    }
}