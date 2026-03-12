namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Base class of a model-driven app page that exposes strongly typed page content.
    /// </summary>
    /// <typeparam name="TModelDrivenAppPageContent">The type representing the contents of the page.</typeparam>
    public abstract class ModelDrivenAppPage<TModelDrivenAppPageContent> : ModelDrivenAppPage, IModelDrivenAppPage<TModelDrivenAppPageContent>
        where TModelDrivenAppPageContent : IModelDrivenAppPageContent
    {
        private readonly IControlFactory controlFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenAppPage{TModelDrivenAppPageContent}"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        protected ModelDrivenAppPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
            this.controlFactory = controlFactory;
        }

        /// <inheritdoc/>
        public TModelDrivenAppPageContent Content => this.controlFactory.CreateCachedInstance<TModelDrivenAppPageContent>(this);
    }
}