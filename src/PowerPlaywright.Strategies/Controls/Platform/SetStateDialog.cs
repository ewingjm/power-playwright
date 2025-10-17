namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
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
        private readonly ILocator closeButton;
        private readonly ILocator cancelButton;
        private readonly ILocator activateButton;
        private readonly ILocator deactivateButton;
        private readonly ILocator toggleMenu;
        private readonly ILocator selectedOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetStateDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public SetStateDialog(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.closeButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Close" });
            this.cancelButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel" });
            this.activateButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Activate" });
            this.deactivateButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Deactivate" });
            this.toggleMenu = this.Container.GetByRole(AriaRole.Combobox, new LocatorGetByRoleOptions { Name = "Status Reason" });
            this.selectedOption = this.toggleMenu.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Selected = true });
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
            await this.Page.WaitForAppIdleAsync();

            var optionText = await this.selectedOption.TextContentAsync();

            return optionText != "---" ? optionText : null;
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
            await this.Page.WaitForAppIdleAsync();

            if (!await this.toggleMenu.ElementExistsAsync())
            {
                throw new PowerPlaywrightException("Unable to find Status Reason control.");
            }

            await this.toggleMenu.SelectOptionAsync(value);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Page.Locator("[data-id='SetStateDialog'][role='dialog'][aria-modal='true']:not([aria-hidden='true'])");
        }
    }
}