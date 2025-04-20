namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    //TODO: Implement Composite using the IFactory once we have the time control (Added in Issue #50).
    /// <summary>
    /// A control strategy for the <see cref="IDateTimeControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class DateTimeControl : PcfControl, IDateTimeControl
    {
        private ILocator dateInput;
        private ILocator timeInput;
        private ILocator timeContainer;
        private ILocator dateContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public DateTimeControl(string name, IAppPage appPage, IControl parent = null)
            : base(name, appPage, parent)
        {
            this.dateContainer = this.Container.Locator($"div[data-lp-id*='MscrmControls.FieldControls.DateControl|{this.Name}.fieldControl._datecontrol']");
            this.timeContainer = this.Container.Locator($"div[data-lp-id*='MscrmControls.FieldControls.TimeControl|{this.Name}.fieldControl._timecontrol']");

            this.dateInput = dateContainer.Locator("input");
            this.timeInput = timeContainer.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<DateTime?> GetValueAsync()
        {
            var dateString = await this.dateInput.InputValueAsync();
            var timeString = string.Empty;

            if (await timeContainer.IsVisibleAsync())
            {
                timeString = await this.timeInput.Last.InputValueAsync();
                Match match = Regex.Match(timeString, @"([01]?[0-9]|2[0-3]):[0-5][0-9]");
                if (match.Success)
                {
                    timeString = match.Value;
                }
            }

            if (DateTime.TryParse($"{dateString} {timeString}", CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
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