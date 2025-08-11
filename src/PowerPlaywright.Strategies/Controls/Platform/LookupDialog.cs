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
    /// A lookup dialog.
    /// </summary>
    [PlatformControlStrategy(0, 0, 0, 0)]
    public class LookupDialog : Control, ILookupDialog
    {
        private readonly IControlFactory controlFactory;

        private readonly ILocator save;
        private readonly ILocator cancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupDialog"/> class.
        /// </summary>
        /// <param name="appPage">The app page.</param>
        /// <param name="controlFactory">The control factory.</param>
        /// <param name="parent">The parent control.</param>
        public LookupDialog(IAppPage appPage, IControlFactory controlFactory, IControl parent = null)
            : base(appPage, parent)
        {
            this.controlFactory = controlFactory;

            this.save = this.Container.Locator("[data-id='lookupDialogSaveBtn']");
            this.cancel = this.Container.Locator("[data-id='lookupDialogCancelBtnFooter']");
        }

        /// <inheritdoc/>
        public ILookup Lookup => this.controlFactory.CreateCachedInstance<ILookup>(this.AppPage, name: "null", parent: this);

        /// <inheritdoc/>
        public async Task CancelAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.cancel.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> IsVisibleAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.Container.IsVisibleAsync();
        }

        /// <inheritdoc/>
        public async Task SaveAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            await this.save.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator("[data-id='lookupDialogRoot'][role='dialog']:not([aria-hidden='true'])");
        }
    }
}
