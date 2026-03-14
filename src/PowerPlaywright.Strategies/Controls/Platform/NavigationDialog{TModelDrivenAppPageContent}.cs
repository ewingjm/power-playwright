namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// Navigation dialog within a model-driven app.
    /// </summary>
    /// <typeparam name="TModelDrivenAppPageContent">The page content type within the dialog.</typeparam>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class NavigationDialog<TModelDrivenAppPageContent> : INavigationDialog<TModelDrivenAppPageContent>
        where TModelDrivenAppPageContent : IModelDrivenAppPageContent
    {
        private readonly IAppPage appPage;
        private readonly IControl parent;
        private readonly TModelDrivenAppPageContent content;

        private readonly ILocator closeButton;
        private readonly ILocator title;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>>
        public NavigationDialog(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
        {
            this.appPage = appPage;
            this.parent = parent;
            this.content = controlFactory.CreateCachedInstance<TModelDrivenAppPageContent>(appPage);

            this.closeButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Close", Exact = true });
            this.title = this.Container.GetByRole(AriaRole.Heading).Nth(0);
        }

        /// <inheritdoc/>
        public TModelDrivenAppPageContent Content => this.content;

        /// <inheritdoc/>
        public IControl Parent => this.parent;

        /// <inheritdoc/>
        public ILocator Container => this.appPage.Page.GetByRole(AriaRole.Dialog);

        /// <inheritdoc/>
        public async Task CloseAsync()
        {
            await this.closeButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<string> GetTitleAsync()
        {
            return await this.title.TextContentAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            return await this.Container.IsVisibleAsync();
        }
    }
}
