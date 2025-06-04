namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A main form.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class MainForm : Control, IMainForm
    {
        private const string RootLocator = "div[data-id='editFormRoot']";
        private readonly IControlFactory controlFactory;

        private readonly ILocator tabList;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="appPage">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public MainForm(IAppPage appPage, IControlFactory controlFactory)
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
        public IFormField GetField(string name)
        {
            return this.controlFactory.CreateInstance<IFormField>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IFormField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<IFormField<TControl>>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public async Task OpenTabAsync(string label)
        {
            await this.tabList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = label }).ClickAsync();
            await this.Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator(RootLocator);
        }
    }
}