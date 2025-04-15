namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Single-line text (Decimal) control class.
    /// </summary>
    public interface IDecimal : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single-line text (Decimal) control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(decimal value);

        /// <summary>
        /// Gets the value of the single-line text (Decimal) control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<decimal?> GetValueAsync();
    }
}