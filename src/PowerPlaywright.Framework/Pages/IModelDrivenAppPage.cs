namespace PowerPlaywright.Framework.Pages
{
    using PowerPlaywright.Framework.Controls.Platform;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public interface IModelDrivenAppPage : IAppPage
    {
        /// <summary>
        /// Gets the site map control.
        /// </summary>
        ISiteMap SiteMap { get; }

        /// <summary>
        /// Gets an object that provides client API functionality.
        /// </summary>
        IClientApi ClientApi { get; }

        /// <summary>
        /// Gets a confirm dialog.
        /// </summary>
        IConfirmDialog ConfirmDialog { get; }

        /// <summary>
        /// Gets an alert dialog.
        /// </summary>
        IAlertDialog AlertDialog { get; }

        /// <summary>
        /// Gets an error dialog.
        /// </summary>
        IErrorDialog ErrorDialog { get; }

        /// <summary>
        /// Gets a deactivate dialog.
        /// </summary>
        IDeactiveDialog DeactiveDialog { get; }
    }
}