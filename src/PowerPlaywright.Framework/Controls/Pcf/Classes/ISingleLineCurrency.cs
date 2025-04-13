namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Single-line text (Currency) control class.
    /// </summary>
    public interface ISingleLineCurrency : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single-line text (Currency) control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(decimal value);

        /// <summary>
        /// Gets the value of the single-line text (Currency) control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<decimal?> GetValueAsync();
    }
}