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
    /// An error dialog.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class ErrorDialog : Control, IErrorDialog
    {
        private readonly ILocator text;
        private readonly ILocator closeButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public ErrorDialog(IAppPage appPage, IControl parent = null)
            : base(appPage, parent)
        {
            this.text = this.Container.Locator("[data-id*='errorDialog_subtitle']");
            this.closeButton = this.Container.Locator("[data-id='errorOkButton']");
        }

        /// <inheritdoc/>
        public async Task CancelAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.closeButton.ClickAndWaitForAppIdleAsync();
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
            return context.Page.Locator("[data-id='errorDialogdialog'][role='dialog'][aria-modal='true']:not([aria-hidden='true'])");
        }
    }
}
