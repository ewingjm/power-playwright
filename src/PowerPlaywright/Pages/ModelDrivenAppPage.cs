namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Model;
    using PowerPlaywright.Model.Controls.Platform;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public abstract class ModelDrivenAppPage : IModelDrivenAppPage
    {
        private ISiteMapControl siteMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenAppPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public ModelDrivenAppPage(IPage page, IControlFactory controlFactory)
        {
            this.Page = page;
            this.ControlFactory = controlFactory;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        public IPage Page { get; }

        /// <inheritdoc/>
        public ISiteMapControl SiteMap
        {
            get
            {
                if (this.siteMap is null)
                {
                    this.siteMap = this.ControlFactory.CreateInstance<ISiteMapControl>(this.Page);
                }

                return this.siteMap;
            }
        }

        /// <summary>
        /// Gets the control factory.
        /// </summary>
        protected IControlFactory ControlFactory { get; }
    }
}