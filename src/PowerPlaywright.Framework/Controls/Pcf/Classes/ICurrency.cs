namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Currency control class.
    /// </summary>
    public interface ICurrency : IPcfControl
    {
        /// <summary>
        /// Sets the value of the Currency control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(decimal value);

        /// <summary>
        /// Gets the value of the Currency control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<decimal?> GetValueAsync();
    }
}