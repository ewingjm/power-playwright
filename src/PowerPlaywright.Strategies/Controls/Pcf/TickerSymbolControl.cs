namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Strategies.Extensions;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="ITickerSymbolControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class TickerSymbolControl : PcfControlInternal, ITickerSymbolControl
    {
        private readonly ILocator input;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickerSymbolControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public TickerSymbolControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.input = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.input.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.input.FocusAsync();
            await this.input.FillAsync(string.Empty);
            await this.input.FillAsync(value);

            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}