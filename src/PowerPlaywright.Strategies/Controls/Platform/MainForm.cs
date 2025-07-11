namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
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
        private readonly ILocator notificationWrapper;
        private readonly ILocator notificationExpandIcon;
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
            this.notificationWrapper = this.Container.Locator("[data-id='notificationWrapper']");
            this.notificationExpandIcon = this.Container.Locator("[data-id='notificationWrapper_expandIcon']");
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
        public async Task<IEnumerable<FormNotification>> GetFormNotificationsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            if (!await this.notificationWrapper.IsVisibleAsync())
            {
                return Enumerable.Empty<FormNotification>();
            }

            if (await this.notificationExpandIcon.IsVisibleAsync() && !await this.notificationWrapper.IsExpandedAsync())
            {
                await this.notificationExpandIcon.ClickAndWaitForAppIdleAsync();
            }

            var notificationListRoot = this.notificationWrapper;
            if (await this.notificationExpandIcon.IsVisibleAsync())
            {
                var notificationFlyoutId = await this.notificationWrapper.GetAttributeAsync(Attributes.AriaControls);
                notificationListRoot = this.Page.Locator($"[id='{notificationFlyoutId}']");
                await notificationListRoot.WaitForAsync();
            }

            var notificationWrappers = await notificationListRoot.Locator("[data-id='notificationList']").Locator("li").AllAsync();

            var notificationTasks = notificationWrappers.Select(
                async n => new FormNotification(
                    await this.GetFormNotificationLevelAsync(n),
                    await n.Locator("[data-id='warningNotification']").TextContentAsync()));

            return await Task.WhenAll(notificationTasks);
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

        private async Task<FormNotificationLevel> GetFormNotificationLevelAsync(ILocator n)
        {
            var symbol = n.Locator("span.symbolFont");
            if (await symbol.IsVisibleAsync())
            {
                var symbolClass = await symbol.GetAttributeAsync(Attributes.Class);

                if (symbolClass.Contains("InformationIcon-symbol"))
                {
                    return FormNotificationLevel.Info;
                }
                else if (symbolClass.Contains("Warning-symbol"))
                {
                    return FormNotificationLevel.Warning;
                }
                else if (symbolClass.Contains("MarkAsLost-symbol"))
                {
                    return FormNotificationLevel.Error;
                }
            }
            else
            {
                var color = await n.Locator("svg").EvaluateAsync<string>("element => window.getComputedStyle(element).color");

                if (color == "rgb(36, 36, 36)")
                {
                    return FormNotificationLevel.Info;
                }
                else if (color == "rgb(143, 78, 0)")
                {
                    return FormNotificationLevel.Warning;
                }
                else if (color == "rgb(188, 47, 50)")
                {
                    return FormNotificationLevel.Error;
                }
            }

            throw new PowerPlaywrightException("Unable to recognise the form notification type.");
        }
    }
}