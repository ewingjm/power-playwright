namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IDurationControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class DurationControl : PcfControlInternal, IDurationControl
    {
        private readonly ILocator input;

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public DurationControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.input = this.Container.GetByRole(AriaRole.Combobox);
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            return await this.input.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.input.ClearAsync();

            if (value != null)
            {
                await this.input.FillAsync(value);
            }

            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}