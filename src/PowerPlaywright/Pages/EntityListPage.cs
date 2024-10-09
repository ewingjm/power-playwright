namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An entity list page.
    /// </summary>
    internal class EntityListPage : ModelDrivenAppPage, IEntityListPage
    {
        // TODO: Implement a strategies based app reference for strings
        private const string GridControlName = "entity_control";

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityListPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public EntityListPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        /// <inheritdoc/>
        public IReadOnlyGrid Grid => this.ControlFactory.CreateInstance<IReadOnlyGrid>(this, GridControlName);
    }
}