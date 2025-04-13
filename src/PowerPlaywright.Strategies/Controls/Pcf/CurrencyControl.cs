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
    /// A control strategy for the <see cref="ICurrencyControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class CurrencyControl : PcfControl, ICurrencyControl
    {
        private readonly ILocator input;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public CurrencyControl(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.input = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<decimal?> GetValueAsync()
        {
            return await this.input.InputValueOrNullAsync<decimal?>();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(decimal value)
        {
            await this.input.FillAsync(value.ToString());
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.CurrencyControl|{this.Name}.fieldControl|']");
        }
    }
}