namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;

    /// <summary>
    /// Navigation dialog within a model-driven app.
    /// </summary>
    /// <typeparam name="TModelDrivenAppPageContent">The page content type within the dialog.</typeparam>
    [PlatformControl]
    public interface INavigationDialog<out TModelDrivenAppPageContent> : IPlatformControl
        where TModelDrivenAppPageContent : IModelDrivenAppPageContent
    {
        /// <summary>
        /// Gets the page content.
        /// </summary>
        TModelDrivenAppPageContent Content { get; }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CloseAsync();

        /// <summary>
        /// Gets the title of the dialog.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> GetTitleAsync();

        /// <summary>
        /// Gets whether the dialog is visible.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> IsVisibleAsync();
    }
}