namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An entity form page.
    /// </summary>
    internal class EntityRecordPage : ModelDrivenAppPage, IEntityRecordPage
    {
        private IMainFormControl form;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRecordPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="controlFactory">The control factory.</param>
        public EntityRecordPage(IPage page, IPageFactory pageFactory, IControlFactory controlFactory)
            : base(page, pageFactory, controlFactory)
        {
        }

        /// <inheritdoc/>
        public IMainFormControl Form => this.form ?? (this.form = this.ControlFactory.CreateInstance<IMainFormControl>(this.Page));
    }
}