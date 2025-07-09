namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A main form.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class MainForm : FormControl, IMainForm
    {
        private readonly IControlFactory controlFactory;

        private readonly ILocator tabList;
        private readonly ILocator tabs;
        private readonly ILocator formReadOnlyNotification;
        private readonly ILocator expandHeaderButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="appPage">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public MainForm(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, controlFactory, parent)
        {
            this.controlFactory = controlFactory ?? throw new ArgumentNullException(nameof(controlFactory));

            this.tabList = this.Container.GetByRole(AriaRole.Tablist);
            this.tabs = this.tabList.GetByRole(AriaRole.Tab);
            this.formReadOnlyNotification = this.Container.Locator("#message-formReadOnlyNotification");
            this.expandHeaderButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "More Header Editable Fields" });
        }

        /// <inheritdoc/>
        public ICommandBar CommandBar => this.controlFactory.CreateCachedInstance<ICommandBar>(this.AppPage, parent: this);

        /// <inheritdoc/>
        public async Task<string> GetActiveTabAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.tabList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Selected = true }).InnerTextAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetAllTabsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.tabs.AllInnerTextsAsync();
        }

        /// <inheritdoc/>
        public new async Task<IEnumerable<IField>> GetFieldsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await base.GetFieldsAsync();
        }

        /// <inheritdoc/>
        public IField GetField(string name)
        {
            return this.controlFactory.CreateCachedInstance<IField>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateCachedInstance<IField<TControl>>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IQuickView GetQuickView(string name)
        {
            return this.controlFactory.CreateCachedInstance<IQuickView>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IDataSet GetDataSet(string name)
        {
            return this.controlFactory.CreateCachedInstance<IDataSet>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IDataSet<TControl> GetDataSet<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateCachedInstance<IDataSet<TControl>>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public async Task<bool> IsDisabledAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.formReadOnlyNotification.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task OpenTabAsync(string label)
        {
            await this.tabList.GetByRole(AriaRole.Tab, new LocatorGetByRoleOptions { Name = label }).ClickAsync();
            await this.Page.WaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<IMainFormHeader> ExpandHeaderAsync()
        {
            if (!await this.IsHeaderExpandedAsync())
            {
                await this.expandHeaderButton.ClickAndWaitForAppIdleAsync();
            }

            return this.controlFactory.CreateCachedInstance<IMainFormHeader>(this.AppPage, parent: this);
        }

        /// <inheritdoc/>
        public async Task CollapseHeaderAsync()
        {
            if (!await this.IsHeaderExpandedAsync())
            {
                return;
            }

            await this.expandHeaderButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.GetByRole(AriaRole.Form).First;
        }

        private async Task<bool> IsHeaderExpandedAsync()
        {
            return await this.expandHeaderButton.IsExpandedAsync();
        }
    }
}