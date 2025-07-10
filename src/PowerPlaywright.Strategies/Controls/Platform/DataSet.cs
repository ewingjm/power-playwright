namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A data set.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class DataSet : Control, IDataSet
    {
        private readonly IControlFactory controlFactory;
        private readonly string name;
        private readonly IControl parent;

        private readonly ILocator viewSelector;
        private readonly ILocator viewsMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSet"/> class.
        /// </summary>
        /// <param name="appPage">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="name">The name of the data set.</param>
        /// <param name="parent">The parent control.</param>
        public DataSet(IAppPage appPage, IControlFactory controlFactory, string name, IControl parent = null)
            : base(appPage)
        {
            this.controlFactory = controlFactory;
            this.name = name;
            this.parent = parent;

            this.viewSelector = this.Container.Locator("button[id*='ViewSelector']");
            this.viewsMenu = this.Page.GetByRole(AriaRole.Menu, new PageGetByRoleOptions { Name = "Views" });
        }

        /// <inheritdoc/>
        public ICommandBar CommandBar => this.controlFactory.CreateCachedInstance<ICommandBar>(this.AppPage, parent: this);

        /// <inheritdoc/>
        public TPcfControl GetControl<TPcfControl>()
            where TPcfControl : IPcfControl
        {
            return this.controlFactory.CreateCachedInstance<TPcfControl>(this.AppPage, this.name, this);
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task SwitchViewAsync(string viewName)
        {
            await this.Page.WaitForAppIdleAsync();

            try
            {
                await this.viewSelector.ClickAndWaitForAppIdleAsync();
            }
            catch (TimeoutException)
            {
                throw new PowerPlaywrightException("The view selector could not be found.");
            }

            try
            {
                await this.viewsMenu.Locator($"//button//label[text()='{viewName.Replace("'", @"\")}']").ClickAndWaitForAppIdleAsync();
            }
            catch (TimeoutException)
            {
                throw new PowerPlaywrightException($"The view selector does not contain the '{viewName}' view.");
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetActiveViewAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.viewSelector.GetAttributeAsync(Attributes.AriaLabel);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-id=\"DataSetHostContainer\"]{(!string.IsNullOrEmpty(this.name) ? $"[id$=\"{this.name}\"]" : string.Empty)}");
        }
    }
}