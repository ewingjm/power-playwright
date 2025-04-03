namespace PowerPlaywright.TestApp.CustomControls
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;

    /// <summary>
    /// A custom control class.
    /// </summary>
    public interface ICustomControlClass : IPcfControl
    {
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(bool value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> GetValueAsync();
    }
}
