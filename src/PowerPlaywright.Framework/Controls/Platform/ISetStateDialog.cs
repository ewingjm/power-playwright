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
        /// Gets the title of the dialog.
        /// </summary>
        /// <returns>The title of the dialog.</returns>
        Task<string> GetTitleAsync();

        /// <summary>
        /// Gets the confirmation text of the dialog.
        /// </summary>
        /// <returns>The confirmation text of the dialog.</returns>
        Task<string> GetTextAsync();

        /// <summary>
        /// Gets the informative statement of for the action the user is about to take.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetActionTextAsync();

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
    }
}