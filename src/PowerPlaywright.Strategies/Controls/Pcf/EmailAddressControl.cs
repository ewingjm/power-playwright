namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IEmailAddressControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class EmailAddressControl : PcfControlInternal, IEmailAddressControl
    {
        private readonly ILocator emailInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public EmailAddressControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.emailInput = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.emailInput.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.emailInput.FocusAsync();
            await this.emailInput.FillAsync(string.Empty);

            if (value != null)
            {
                await this.emailInput.FillAsync(value);
            }

            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}