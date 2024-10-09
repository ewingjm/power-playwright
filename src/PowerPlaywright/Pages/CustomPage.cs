namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;

    /// <summary>s
    /// A custom page.
    /// </summary>
    internal class CustomPage : ModelDrivenAppPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public CustomPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }
    }
}