namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IDecimalNumberControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class DecimalNumberControl : PcfControlInternal, IDecimalNumberControl
    {
        private readonly ILocator input;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalNumberControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public DecimalNumberControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.input = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<decimal?> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.input.InputValueOrNullAsync<decimal?>();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(decimal? value)
        {
            await this.input.FocusAsync();
            await this.input.FillAsync(value.ToString());
            await this.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}