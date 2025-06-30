namespace PowerPlaywright.Strategies.Controls.Platform
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// A main form.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class MainForm : Control, IMainForm
    {
        private readonly IControlFactory controlFactory;

        private readonly ILocator tabList;
        private readonly ILocator tabs;
        private readonly ILocator formReadOnlyNotification;

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
            this.tabs = this.tabList.GetByRole(AriaRole.Tab);
            this.formReadOnlyNotification = this.Container.Locator("#message-formReadOnlyNotification");
        }

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
        public async Task<IEnumerable<IField>> GetFieldsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var fieldType = this.controlFactory.GetRedirectedType<IField>();
            var fieldControlName = fieldType.GetCustomAttribute<PcfControlAttribute>().Name;

            var fieldLocators = await this.Container.Locator($"div[data-lp-id*='{fieldControlName}']").AllAsync();

            var lpIdTasks = fieldLocators
                .Select(field => field.GetAttributeAsync(Attributes.DataLpId))
                .ToArray();

            var lpIds = await Task.WhenAll(lpIdTasks);

            var fields = lpIds
                .Select(lpId =>
                {
                    var fieldName = lpId.Split('|')[1];
                    return this.controlFactory.CreateInstance<IField>(this.AppPage, fieldName, this);
                })
                .ToList();

            return fields;
        }

        /// <inheritdoc/>
        public IField GetField(string name)
        {
            return this.controlFactory.CreateInstance<IField>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<IField<TControl>>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IQuickView GetQuickView(string name)
        {
            return this.controlFactory.CreateInstance<IQuickView>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IDataSet GetDataSet(string name)
        {
            return this.controlFactory.CreateInstance<IDataSet>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IDataSet<TControl> GetDataSet<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.controlFactory.CreateInstance<IDataSet<TControl>>(this.AppPage, name, this);
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
        protected override ILocator GetRoot(ILocator context)
        {
            return context.GetByRole(AriaRole.Form);
        }
    }
}