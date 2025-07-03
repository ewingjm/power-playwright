namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IActionInput"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class ActionInput : PcfControlInternal, IActionInput
    {
        private readonly ILocator controlInput;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionInput"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public ActionInput(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.controlInput = this.Container.Locator("input");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.controlInput.InputValueOrNullAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.controlInput.FocusAsync();
            await this.controlInput.FillAsync(string.Empty);
            await this.controlInput.FillAsync(value);
            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }
    }
}