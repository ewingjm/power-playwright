namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A confirm dialog.
    /// </summary>
    [PlatformControl]
    public interface IConfirmDialog : IPlatformControl
    {
        /// <summary>
        /// Gets whether the dialog is visible.
        /// </summary>
        /// <returns>A boolean indicating whether the dialog is visible.</returns>
        Task<bool> IsVisibleAsync();

        /// <summary>
        /// Confirm the action on the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ConfirmAsync();

        /// <summary>
        /// Confirm the the dialog that navigates to another page.
        /// </summary>
        /// <typeparam name="TModelDrivenAppPage">The type of page.</typeparam>
        /// <returns>The page.</returns>
        Task<TModelDrivenAppPage> ConfirmAsync<TModelDrivenAppPage>()
            where TModelDrivenAppPage : IModelDrivenAppPage;

        /// <summary>
        /// Cancel the action on the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CancelAsync();

        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        /// <returns>The title of the dialog.</returns>
        Task<string> GetTitleAsync();

        /// <summary>
        /// Gets the subtitle of the dialog.
        /// </summary>
        /// <returns>The subtitle of the dialog.</returns>
        Task<string> GetSubtitleAsync();

        /// <summary>
        /// Gets the text of the dialog.
        /// </summary>
        /// <returns>The text of the dialog.</returns>
        Task<string> GetTextAsync();
    }
}
