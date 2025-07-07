namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An alert dialog.
    /// </summary>
    [PlatformControl]
    public interface IAlertDialog : IPlatformControl
    {
        /// <summary>
        /// Gets whether the dialog is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the dialog is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Confirm the the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ConfirmAsync();

        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        /// <returns>The title of the dialog.</returns>
        Task<string> GetTitleAsync();

        /// <summary>
        /// Gets the text of the dialog.
        /// </summary>
        /// <returns>The text of the dialog.</returns>
        Task<string> GetTextAsync();
    }
}
