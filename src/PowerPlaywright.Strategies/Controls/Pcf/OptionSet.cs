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
    public class OptionSet : PcfControlInternal, IOptionSet
    {
        private readonly ILocator toggleMenu;
        private readonly ILocator selectedOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSet"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public OptionSet(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.toggleMenu = this.Container.GetByRole(AriaRole.Combobox);
            this.selectedOption = this.toggleMenu.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Selected = true });
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var optionText = await this.selectedOption.TextContentAsync(new LocatorTextContentOptions { Timeout = 0 });

            return optionText != "---" ? optionText : null;
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.toggleMenu.SelectOptionAsync(value);
        }
    }
}
