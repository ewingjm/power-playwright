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
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IDateTimeControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class DateTimeControl : PcfControlInternal, IDateTimeControl
    {
        private readonly ILocator dateInput;
        private readonly ILocator timeInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public DateTimeControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.dateInput = this.Container.GetByRole(AriaRole.Combobox, new LocatorGetByRoleOptions { Name = "Date of" });
            this.timeInput = this.Container.GetByRole(AriaRole.Combobox, new LocatorGetByRoleOptions { Name = "Time of" });
        }

        /// <inheritdoc/>
        public async Task<DateTime?> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var dateString = await this.dateInput.InputValueOrNullAsync();

            if (string.IsNullOrEmpty(dateString))
            {
                return null;
            }

            var timeString = await timeInput.IsVisibleAsync() ? await this.timeInput.InputValueOrNullAsync() : null;

            return DateTime.Parse($"{dateString} {timeString}".TrimEnd());
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(DateTime? value)
        {
            // TODO: Set current thread culture equal to the culture of the app user. Currently, this will only work if the app user culture is the same as the current culture.
            await this.dateInput.FocusAsync();
            await this.dateInput.FillAsync(value.Value.ToShortDateString());
            await this.Container.ClickAndWaitForAppIdleAsync();
            await this.timeInput.FocusAsync();
            await this.timeInput.FillAsync(value.Value.ToShortTimeString());
            await this.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}