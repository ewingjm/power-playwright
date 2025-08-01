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
        /// Gets the global search control.
        /// </summary>
        IGlobalSearch Search { get; }

        /// <summary>
        /// Gets a confirm dialog.
        /// </summary>
        IConfirmDialog ConfirmDialog { get; }

        /// <summary>
        /// Gets an alert dialog.
        /// </summary>
        IAlertDialog AlertDialog { get; }
    }
}