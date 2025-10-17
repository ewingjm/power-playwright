namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A deactivate dialog.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class SetStateDialog : Control, ISetStateDialog
    {
        private readonly IChoice choice;
        private readonly ILocator closeButton;
        private readonly ILocator cancelButton;
        private readonly ILocator activateButton;
        private readonly ILocator deactivateButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetStateDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public SetStateDialog(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.choice = controlFactory.CreateCachedInstance<IChoice>(appPage, "status_id", parent);

            this.closeButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Close" });
            this.cancelButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel" });
            this.activateButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Activate" });
            this.deactivateButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Deactivate" });
        }

        /// <inheritdoc/>
        public async Task ActivateAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.activateButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task CancelAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.cancelButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task CloseAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.closeButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task DeactivateAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.deactivateButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            return await this.choice.GetValueAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.choice.SetValueAsync(value);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Page.Locator("[data-id='SetStateDialog'][role='dialog'][aria-modal='true']:not([aria-hidden='true'])");
        }
    }
}