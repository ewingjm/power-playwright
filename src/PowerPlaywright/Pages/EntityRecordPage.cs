namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Model;
    using PowerPlaywright.Model.Controls.Platform;

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
        /// <param name="controlFactory">The control factory.</param>
        public EntityRecordPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        /// <inheritdoc/>
        public IMainFormControl Form => this.form ?? (this.form = this.ControlFactory.CreateInstance<IMainFormControl>(this.Page));
    }
}