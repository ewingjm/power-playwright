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
        private readonly IPlatformReference platformReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityListPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="platformReference">The platform reference.</param>
        public EntityListPage(IPage page, IControlFactory controlFactory, IPlatformReference platformReference)
            : base(page, controlFactory)
        {
            this.platformReference = platformReference ?? throw new ArgumentNullException(nameof(platformReference));
        }

        public IDataSet DataSet => this.GetControl<IDataSet>(string.Empty);
    }
}