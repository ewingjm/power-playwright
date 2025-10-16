namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// Deactivate dialog.
    /// </summary>
    [PlatformControl]
    public interface IDeactiveDialog : ISetStateDialog
    {
        /// <summary>
        /// Click the deactivate button.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeactivateAsync();

        /// <summary>
        /// Sets the value of the choice control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetValueAsync(string value);
    }
}