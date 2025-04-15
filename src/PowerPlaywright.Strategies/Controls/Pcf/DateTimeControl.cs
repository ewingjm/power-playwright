namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System;
    using System.Globalization;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IDateTimeControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class DateTimeControl : PcfControl, IDateTimeControl
    {
        private readonly ILocator dateInput;
        private readonly ILocator timeInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public DateTimeControl(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            var dateContainer = this.Container.Locator($"div[data-lp-id*='MscrmControls.FieldControls.DateControl|{this.Name}.fieldControl._datecontrol']");
            var timeContainer = this.Container.Locator($"div[data-lp-id*='MscrmControls.FieldControls.TimeControl|{this.Name}.fieldControl._timecontrol']");

            this.dateInput = dateContainer.Locator("input");
            this.timeInput = timeContainer.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<DateTime?> GetValueAsync()
        {
            var dateString = await this.dateInput.InputValueAsync();
            var timeString = await this.timeInput.InputValueAsync();

            var combinedString = $"{dateString} {timeString}";

            // Try parsing with current culture
            if (DateTime.TryParse(combinedString, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
            {
                return dateTime;
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(DateTime? value)
        {
            var dateTask = this.dateInput.FillAsync(value.Value.ToShortDateString());
            var timeTask = this.timeInput.FillAsync(value.Value.ToShortTimeString());

            await Task.WhenAll(dateTask, timeTask);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='MscrmControls.FieldControls.DateTimeControl|{this.Name}.fieldControl']");
        }
    }
}