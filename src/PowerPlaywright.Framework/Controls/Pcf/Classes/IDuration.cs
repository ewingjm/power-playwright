namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Duration control class.
    /// </summary>
    public interface IDuration : IPcfControl
    {
        /// <summary>
        /// Sets the value of the Duration control.The value must be in the following format: x minute(s)/hour(s)/day(s).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the Duration control. The value will be in the following format: x minute(s)/hour(s)/day(s).
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}