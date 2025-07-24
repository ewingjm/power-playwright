namespace PowerPlaywright.Strategies.Controls.Platform
{
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
    using PowerPlaywright.Framework.Model;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A quick create form.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class QuickCreateForm : FormControl, IQuickCreateForm
    {
        private readonly ILocator saveAndCloseButton;
        private readonly ILocator cancelButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickCreateForm"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public QuickCreateForm(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, controlFactory, parent)
        {
            this.saveAndCloseButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Save and Close", Exact = true });
            this.cancelButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel", Exact = true });
        }

        /// <inheritdoc/>
        public IField<TControl> GetField<TControl>(string name)
            where TControl : IPcfControl
        {
            return this.ControlFactory.CreateCachedInstance<IField<TControl>>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public IField GetField(string name)
        {
            return this.ControlFactory.CreateCachedInstance<IField>(this.AppPage, name, this);
        }

        /// <inheritdoc/>
        public new async Task<IEnumerable<IField>> GetFieldsAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await base.GetFieldsAsync();
        }

        /// <inheritdoc/>
        public new Task<IEnumerable<FormNotification>> GetFormNotificationsAsync()
        {
            return base.GetFormNotificationsAsync();
        }

        /// <inheritdoc/>
        public async Task CancelAsync()
        {
            await this.cancelButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task SaveAndCloseAsync()
        {
            await this.saveAndCloseButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return this.Page.GetByRole(AriaRole.Dialog, new PageGetByRoleOptions { Name = "Quick Create: " });
        }
    }
}
