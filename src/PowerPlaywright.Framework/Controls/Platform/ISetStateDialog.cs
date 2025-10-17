namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// Set state dialog.
    /// </summary>
    [PlatformControl]
    public interface ISetStateDialog : IPlatformControl
    {
        /// <summary>
        /// Gets whether the dialog is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the dialog is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Close the the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CloseAsync();

        /// <summary>
        /// Cancel the the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CancelAsync();

        /// <summary>
        /// Confirms the dialog action.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ConfirmAsync();

        /// <summary>
        /// Gets the value of the choice control.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetValueAsync();

        /// <summary>
        /// Sets the status reason value of the choice control.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SetStatusReasonAsync(string value);
    }
}