namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A base class for all form controls.
    /// </summary>
    public abstract class FormControl : Control
    {
        private readonly ILocator notificationWrapper;
        private readonly ILocator notificationExpandIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormControl"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        protected FormControl(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.notificationWrapper = this.Container.Locator("[data-id='notificationWrapper']");
            this.notificationExpandIcon = this.Container.Locator("[data-id='notificationWrapper_expandIcon']");

            this.ControlFactory = controlFactory;
        }

        /// <summary>
        /// Gets the control factory.
        /// </summary>
        protected IControlFactory ControlFactory { get; }

        /// <summary>
        /// Gets the fields on the form.
        /// </summary>
        /// <returns>All fields on the form.</returns>
        protected async Task<IEnumerable<IField>> GetFieldsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var fieldLocators = await this.Container.Locator($"div[data-lp-id*='{this.GetFieldControlName()}']").AllAsync();

            var lpIdTasks = fieldLocators
                .Select(field => field.GetAttributeAsync(Attributes.DataLpId))
                .ToArray();

            var lpIds = await Task.WhenAll(lpIdTasks);

            var fields = lpIds
                .Select(lpId =>
                {
                    var fieldName = lpId.Split('|')[1];
                    return this.ControlFactory.CreateCachedInstance<IField>(this.AppPage, fieldName, this);
                })
                .ToList();

            return fields;
        }

        /// <summary>
        /// Get form notifications.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task<IEnumerable<FormNotification>> GetFormNotificationsAsync()
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

        private string GetFieldControlName()
        {
            return this.ControlFactory
                .GetRedirectedType<IField>(this.AppPage)
                .GetCustomAttribute<PcfControlAttribute>().Name;
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
