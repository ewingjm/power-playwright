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

    /// <summary>
    /// A sitemap control.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class SiteMapControl : Control, ISiteMapControl
    {
        private const string RootLocator = "div[data-id='navbar-container']";
        private const string AreaSwitcherLocator = "#areaSwitcherId";
        private const string AreaSwitcherFlyoutLocator = "div[data-lp-id='sitemap-area-switcher-flyout']";

        private readonly IPageFactory pageFactory;

        private readonly ILocator areaSwitcher;
        private readonly ILocator areaSwitcherFlyout;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="pageFactory">The page factory.</param>
        public SiteMapControl(IAppPage appPage, IPageFactory pageFactory)
            : base(appPage)
        {
            this.pageFactory = pageFactory;

            this.areaSwitcher = this.Container.Locator(AreaSwitcherLocator);
            this.areaSwitcherFlyout = this.Page.Locator(AreaSwitcherFlyoutLocator);
        }

        /// <inheritdoc/>
        public async Task<TPage> OpenPageAsync<TPage>(string area, string group, string page)
            where TPage : IModelDrivenAppPage
        {
            if (!await this.IsAreaActiveAsync(area))
            {
                await this.OpenAreaAsync(area);
            }

            await this.Page.WaitForAppIdleAsync();

            var pageItem = this.GetPageLocator(group, page);
            if (!await pageItem.IsVisibleAsync())
            {
                throw new NotFoundException($"Unable to find page {pageItem} in the {group} group.");
            }

            await pageItem.ClickAsync();
            await this.Page.WaitForAppIdleAsync();

            return await this.pageFactory.CreateInstanceAsync<TPage>(this.Page);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator(RootLocator);
        }

        private async Task OpenAreaAsync(string area)
        {
            await this.areaSwitcher.ClickAsync();
            await this.Page.WaitForAppIdleAsync();

            var areaFlyoutItem = this.areaSwitcherFlyout.GetByRole(AriaRole.Menuitemradio, new LocatorGetByRoleOptions { Name = area });

            if (!await areaFlyoutItem.IsVisibleAsync())
            {
                throw new NotFoundException($"Unable to find an area named {area} on the sitemap area switcher flyout.");
            }

            await areaFlyoutItem.ClickAsync();
        }

        private async Task<bool> IsAreaActiveAsync(string area)
        {
            await this.Page.WaitForAppIdleAsync();

            if (!await this.areaSwitcher.IsVisibleAsync())
            {
                return true;
            }

            var label = await this.areaSwitcher.GetAttributeAsync("aria-label");

            return label == $"{area} (change area)";
        }

        private ILocator GetGroupLocator(string group)
        {
            return this.Container.GetByRole(AriaRole.Group, new LocatorGetByRoleOptions() { Name = group });
        }

        private ILocator GetPageLocator(string group, string page)
        {
            return this.GetGroupLocator(group)
                .GetByRole(AriaRole.Treeitem, new LocatorGetByRoleOptions { Name = page });
        }
    }
}