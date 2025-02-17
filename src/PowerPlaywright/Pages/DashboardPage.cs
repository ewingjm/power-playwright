﻿namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A dashboard page.
    /// </summary>
    internal class DashboardPage : ModelDrivenAppPage, IDashboardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public DashboardPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }
    }
}