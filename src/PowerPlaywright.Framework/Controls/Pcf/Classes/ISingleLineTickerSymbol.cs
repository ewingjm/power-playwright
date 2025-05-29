namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Single-line text (ticker symbol) control class.
    /// </summary>
    public interface ISingleLineTickerSymbol : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single-line text (ticker symbol) control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the single-line text (ticker symbol) control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}