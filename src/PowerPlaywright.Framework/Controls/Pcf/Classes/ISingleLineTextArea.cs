namespace PowerPlaywright.Framework.Controls.Pcf.Classes
{
    using System.Threading.Tasks;

    /// <summary>
    /// Single-line text (TextArea) control class.
    /// </summary>
    public interface ISingleLineTextArea : IPcfControl
    {
        /// <summary>
        /// Sets the value of the single-line text (TextArea) control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);

        /// <summary>
        /// Gets the value of the single-line text (TextArea) control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();
    }
}