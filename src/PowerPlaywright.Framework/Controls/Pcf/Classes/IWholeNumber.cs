namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Single-line text (Whole Number) control class.
    /// </summary>
    public interface IWholeNumber : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single-line text (Whole Number) control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(int value);

        /// <summary>
        /// Gets the value of the single-line text (Whole Number) control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<int?> GetValueAsync();
    }
}