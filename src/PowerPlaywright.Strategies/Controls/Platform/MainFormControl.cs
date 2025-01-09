namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// A main form.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class MainFormControl : Control, IMainFormControl
    {
        private const string RootLocator = "div[data-id='editFormRoot']";
        private readonly IControlFactory controlFactory;

        private readonly ILocator tabList;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFormControl"/> class.
        /// </summary>
        /// <param name="appPage">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public MainFormControl(IAppPage appPage, IControlFactory controlFactory)
            : base(appPage)
        {
            this.controlFactory = controlFactory ?? throw new ArgumentNullException(nameof(controlFactory));

            this.tabList = this.Container.GetByRole(AriaRole.Tablist);
        }

        /// <inheritdoc/>
        public Task<string> GetActiveTabAsync()
        {
            return this.tabList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Selected = true }).InnerTextAsync();
        }

        /// <inheritdoc/>
        public async Task OpenTabAsync(string label)
        {
            await this.tabList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = label }).ClickAsync();
            await this.Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public TControl GetControl<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<TControl>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator(RootLocator);
        }
    }
}