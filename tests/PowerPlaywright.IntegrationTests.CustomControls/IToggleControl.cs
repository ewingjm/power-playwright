namespace PowerPlaywright.TestApp.CustomControls
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;

    /// <summary>
    /// An interface for the MscrmControls.FieldControls.ToggleControl control.
    /// </summary>
    [PcfControl("MscrmControls.FieldControls.ToggleControl")]
    public interface IToggleControl : IPcfControl
    {
        /// <summary>
        /// Sets the value of the toggle.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(bool value);

        /// <summary>
        /// Gets the value of the toggle.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> GetValueAsync();
    }
}
