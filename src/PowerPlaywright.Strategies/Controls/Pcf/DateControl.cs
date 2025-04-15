namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IDateControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class DateControl : PcfControl, IDateControl
    {
        private readonly ILocator input;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public DateControl(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.input = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<DateTime?> GetValueAsync()
        {
            return await this.input.InputValueOrNullAsync<DateTime?>();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(DateTime? value)
        {
            await this.input.FillAsync(value.ToString());
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.DateControl|{this.Name}.fieldControl._datecontrol']");
        }
    }
}