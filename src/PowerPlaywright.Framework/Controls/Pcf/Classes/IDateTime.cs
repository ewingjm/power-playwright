namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Date and time control class.
    /// </summary>
    public interface IDateTime : IPcfControl
    {
        /// <summary>
        /// Sets the value of the date and time control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(DateTime? value);

        /// <summary>
        /// Gets the value of the date and time control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<DateTime?> GetValueAsync();
    }
}