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

    /// <summary>
    /// A control strategy for the <see cref="IOptionSetControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class CheckboxControl : PcfControlInternal, ICheckboxControl
    {
        private readonly ILocator comboBox;
        private readonly ILocator textBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckboxControl"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public CheckboxControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.comboBox = this.Container.GetByRole(AriaRole.Combobox);
            this.textBox = this.Container.GetByRole(AriaRole.Textbox);
        }

        /// <inheritdoc/>
        public async Task<bool> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var value = await this.comboBox.InputValueAsync();

            return value == "1";
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(bool value)
        {
            await this.Page.WaitForAppIdleAsync();

            if (await this.textBox.IsVisibleAsync())
            {
                await this.textBox.ClickAsync();
            }

            await this.comboBox.SelectOptionAsync(value ? "1" : "0");
        }
    }
}
