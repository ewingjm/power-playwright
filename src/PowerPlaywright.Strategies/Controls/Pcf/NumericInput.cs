namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Strategies.Extensions;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;

    /// <summary>
    /// A control strategy for the <see cref="INumericInput"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class NumericInput : PcfControl, INumericInput
    {
        private readonly ILocator numericInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericInput"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public NumericInput(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.numericInput = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        async Task<decimal?> ICurrency.GetValueAsync()
        {
            return await this.numericInput.InputValueOrNullAsync<decimal?>();
        }

        /// <inheritdoc/>
        async Task<decimal?> IDecimal.GetValueAsync()
        {
            return await this.numericInput.InputValueOrNullAsync<decimal?>();
        }

        /// <inheritdoc/>
        async Task<double?> IFloatingPointNumber.GetValueAsync()
        {
            return await this.numericInput.InputValueOrNullAsync<double?>();
        }

        /// <inheritdoc/>
        async Task<int?> IWholeNumber.GetValueAsync()
        {
            return await this.numericInput.InputValueOrNullAsync<int?>();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(float value)
        {
            await this.numericInput.FillAsync(value.ToString());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(decimal value)
        {
            await this.numericInput.FillAsync(value.ToString());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(int value)
        {
            await this.numericInput.FillAsync(value.ToString());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(double value)
        {
            await this.numericInput.FillAsync(value.ToString());
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"//div[contains(@data-lp-id,'PowerApps.CoreControls.NumericInput|{this.Name}.fieldControl')]");
        }
    }
}