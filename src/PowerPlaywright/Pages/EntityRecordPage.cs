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
        public IMainFormControl Form => this.ControlFactory.CreateInstance<IMainFormControl>(this);
    }
}