namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Lookup control class.
    /// </summary>
    public interface ISingleLineUrl : IPcfControl
    {
        /// <summary>
        /// Sets the value of the lookup.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the lookup.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}