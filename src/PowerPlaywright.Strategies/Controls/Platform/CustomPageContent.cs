namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Custom page content.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class CustomPageContent : ICustomPageContent
    {
        private readonly IAppPage appPage;
        private readonly IControl parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomPageContent"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public CustomPageContent(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
        {
            this.appPage = appPage;
            this.parent = parent;
        }

        /// <inheritdoc/>
        public IControl Parent => this.parent;

        /// <inheritdoc/>
        public ILocator Container => this.parent is null ? this.appPage.Page.GetByRole(AriaRole.Main) : this.parent.Container.Locator("div[id*=\"dialogPageContainer\"]").Last;
    }
}