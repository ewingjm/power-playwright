namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="INumericInput"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class NumericInput : PcfControlInternal, INumericInput
    {
        private readonly ILocator numericInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericInput"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public NumericInput(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.numericInput = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        async Task<decimal?> ICurrency.GetValueAsync()
        {
            var value = await this.GetValueAsync<string>();

            return value != null ? Convert.ToDecimal(Regex.Replace(value, @"[^\d.,]", "")) : (decimal?)null;
        }

        /// <inheritdoc/>
        async Task<decimal?> IDecimalNumber.GetValueAsync()
        {
            return await this.GetValueAsync<decimal?>();
        }

        /// <inheritdoc/>
        async Task<double?> IFloatingPointNumber.GetValueAsync()
        {
            return await this.GetValueAsync<double?>();
        }

        /// <inheritdoc/>
        async Task<int?> IWholeNumber.GetValueAsync()
        {
            return await this.GetValueAsync<int?>();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(float? value)
        {
            await this.SetValueAsync(value?.ToString());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(decimal? value)
        {
            await this.SetValueAsync(value?.ToString());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(int? value)
        {
            await this.SetValueAsync(value?.ToString());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(double? value)
        {
            await this.SetValueAsync(value?.ToString());
        }

        private async Task<TType> GetValueAsync<TType>()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.numericInput.InputValueOrNullAsync<TType>();
        }

        private async Task SetValueAsync(string value)
        {
            await this.numericInput.FocusAsync();
            await this.numericInput.FillAsync(string.Empty);
            await this.numericInput.FillAsync(value);
            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}