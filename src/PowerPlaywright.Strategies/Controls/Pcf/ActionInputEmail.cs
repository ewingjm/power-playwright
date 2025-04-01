namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IActionInputEmail"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class ActionInputEmail : PcfControl, IActionInputEmail
    {
        private readonly ILocator emailInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInputEmail"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public ActionInputEmail(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.emailInput = this.Container.Locator($"input[data-id='{this.Name}.fieldControl-mail-text-input']");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            return await this.emailInput.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.emailInput.FillAsync(value);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"//div[contains(@data-lp-id,'PowerApps.CoreControls.ActionInput|{this.Name}.fieldControl')]");
        }
    }
}