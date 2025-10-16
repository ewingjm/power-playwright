namespace PowerPlaywright.Strategies.Controls.Platform
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
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
    public class DeactivateDialog : Control, IDeactiveDialog
    {
        private readonly ILocator title;
        private readonly ILocator description;
        private readonly ILocator message;
        private readonly ILocator closeButton;
        private readonly ILocator cancelButton;
        private readonly ILocator deactivateButton;
        private readonly ILocator toggleMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeactivateDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public DeactivateDialog(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.title = this.Container.GetByRole(AriaRole.Heading);
            this.description = this.Container.Locator("[data-id='description_id']");
            this.message = this.Container.Locator("[data-id='message_id']");

            this.closeButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Close" });
            this.cancelButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Cancel" });
            this.deactivateButton = this.Container.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Deactivate" });
            this.toggleMenu = this.Container.GetByRole(AriaRole.Combobox, new LocatorGetByRoleOptions { Name = "Status Reason" });
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
        public async Task<string> GetActionTextAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var text = await this.message.InnerTextAsync();
            return text.Replace("\n\n", "\n");
        }

        /// <inheritdoc/>
        public async Task<string> GetTextAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var text = await this.description.InnerTextAsync();
            return text.Replace("\n\n", "\n");
        }

        /// <inheritdoc/>
        public async Task<string> GetTitleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.title.InnerTextAsync();
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
            await this.toggleMenu.SelectOptionAsync(value);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Page.Locator("[data-id='SetStateDialog'][role='dialog'][aria-modal='true']:not([aria-hidden='true'])");
        }
    }
}