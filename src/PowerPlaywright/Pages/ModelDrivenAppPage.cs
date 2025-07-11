namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public abstract class ModelDrivenAppPage : AppPage, IModelDrivenAppPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenAppPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public ModelDrivenAppPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        /// <inheritdoc/>
        public ISiteMap SiteMap => this.GetControl<ISiteMap>();

        /// <inheritdoc/>
        public IClientApi ClientApi => this.GetControl<IClientApi>();

        /// <inheritdoc/>
        public IConfirmDialog ConfirmDialog => this.GetControl<IConfirmDialog>();

        /// <inheritdoc/>
        public IAlertDialog AlertDialog => this.GetControl<IAlertDialog>();
    }
}