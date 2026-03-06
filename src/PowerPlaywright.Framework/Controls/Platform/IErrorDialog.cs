namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// An error dialog.
    /// </summary>
    [PlatformControl]
    public interface IErrorDialog : IPlatformControl
    {
        /// <summary>
        /// Gets whether the dialog is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the dialog is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Closes the error dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CloseAsync();

        /// <summary>
        /// Gets the text of the error dialog.
        /// </summary>
        /// <returns>The text of the dialog.</returns>
        Task<string> GetTextAsync();
    }
}
