namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Controls.Pcf.Classes;

    /// <summary>
    /// An entity list page.
    /// </summary>
    internal class EntityListPage : ModelDrivenAppPage, IEntityListPage
    {
        private const string GridControlName = "entity_control";

        private IReadOnlyGrid grid;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityListPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory</param>
        public EntityListPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        /// <inheritdoc/>
        public IReadOnlyGrid Grid => this.grid ?? (this.grid = this.ControlFactory.CreateInstance<IReadOnlyGrid>(this.Page, GridControlName));
    }
}