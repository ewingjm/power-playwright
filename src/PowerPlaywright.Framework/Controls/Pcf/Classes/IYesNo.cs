namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Yes/No control class.
    /// </summary>
    public interface IYesNo : IPcfControl
    {
        /// <summary>
        /// Sets the value of the Yes/No control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(bool value);

        /// <summary>
        /// Gets the value of the Yes/No control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> GetValueAsync();
    }
}