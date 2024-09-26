namespace PowerPlaywright.Pages
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public abstract class ModelDrivenAppPage : BasePage, IModelDrivenAppPage
    {
        private readonly IPageFactory pageFactory;

        private ISiteMapControl siteMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenAppPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        public ModelDrivenAppPage(IPage page, IPageFactory pageFactory, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
            this.pageFactory = pageFactory;
        }

        /// <inheritdoc/>
        public ISiteMapControl SiteMap => this.siteMap ?? (this.siteMap = this.ControlFactory.CreateInstance<ISiteMapControl>(this.Page));

        /// <inheritdoc/>
        public async Task<IEntityRecordPage> NavigateToRecordAsync(string entityName, Guid entityId)
        {
            await this.Page.EvaluateAsync("async ({ entityName, entityId } ) => { await Xrm.Navigation.navigateTo({ pageType: 'entityrecord', entityName: entityName, entityId: entityId }) } ", new { entityName, entityId });

            return await this.pageFactory.CreateInstanceAsync<IEntityRecordPage>(this.Page);
        }
    }
}