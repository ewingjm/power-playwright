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
    /// A confirm dialog.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class ConfirmDialog : Control, IConfirmDialog
    {
        private readonly ILocator title;
        private readonly ILocator subtitle;
        private readonly ILocator text;
        private readonly ILocator confirmButton;
        private readonly ILocator cancelButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public ConfirmDialog(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.title = this.Container.GetByRole(AriaRole.Heading);
            this.subtitle = this.Container.Locator("[data-id='errorDialog_subtitle']");
            this.text = this.Container.Locator("[id*='modalDialogBody']");
            this.confirmButton = this.Container.Locator("[data-id='confirmButton']");
            this.cancelButton = this.Container.Locator("[data-id='cancelButton']");
        }

        /// <inheritdoc/>
        public async Task CancelAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.cancelButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task ConfirmAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.confirmButton.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<string> GetTitleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.title.InnerTextAsync();
        }

        /// <inheritdoc/>
        public async Task<string> GetSubtitleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.subtitle.InnerTextAsync();
        }

        /// <inheritdoc/>
        public async Task<string> GetTextAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var text = await this.text.InnerTextAsync();
            return text.Replace("\n\n", "\n");
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Page.Locator("[role='dialog'][aria-modal='true']:not([aria-hidden='true'])");
        }
    }
}
