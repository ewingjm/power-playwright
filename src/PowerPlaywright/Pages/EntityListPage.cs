namespace PowerPlaywright.Pages
{
    using System;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An entity list page.
    /// </summary>
    internal class EntityListPage : ModelDrivenAppPage, IEntityListPage
    {
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
        public IDataSet DataSet => this.GetControl<IDataSet>(string.Empty);
    }
}