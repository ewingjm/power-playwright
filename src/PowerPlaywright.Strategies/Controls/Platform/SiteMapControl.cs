namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Model.Controls.Platform;
    using PowerPlaywright.Model.Controls.Platform.Attributes;

    /// <summary>
    /// A sitemap control.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class SiteMapControl : Control, ISiteMapControl
    {
        private const string ContainerSelector = "div[data-id='navbar-container']";

        private readonly ILocator container;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteMapControl"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public SiteMapControl(IPage page)
            : base(page)
        {
            this.container = page.Locator(ContainerSelector);
        }

        /// <inheritdoc/>
        public Task OpenPageAsync(string area, string page)
        {
            return this.GetPageLocator(area, page).ClickAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetContainer()
        {
            return this.Page.Locator(ContainerSelector);
        }

        private ILocator GetAreaLocator(string area)
        {
            return this.container.GetByRole(AriaRole.Heading, new LocatorGetByRoleOptions() { Name = area });
        }

        private ILocator GetPageLocator(string area, string page)
        {
            return this.GetAreaLocator(area).GetByText(page, new LocatorGetByTextOptions { Exact = true });
        }
    }
}
