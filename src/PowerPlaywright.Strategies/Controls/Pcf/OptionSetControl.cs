namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IOptionSetControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class OptionSetControl : PcfControl, IOptionSetControl
    {
        private readonly ILocator toggleMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdMSPicklistControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public OptionSetControl(string name, IAppPage appPage, IControl parent = null) : base(name, appPage, parent)
        {
            this.toggleMenu = this.Container.Locator("button[role='combobox']");
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await Page.WaitForAppIdleAsync();

            return await toggleMenu.GetAttributeAsync("value") ?? null;
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string optionValue)
        {
            await this.Page.WaitForAppIdleAsync();
            await this.Container.ClickAsync();
            await toggleMenu.ClickIfVisibleAsync(hoverOver: true, scrollIntoView: true);

            var option = this.Page.Locator("div[role='option']").Filter(new LocatorFilterOptions
            {
                HasTextString = optionValue
            }).First;

            await option.ClickIfVisibleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='PowerApps.CoreControls.OptionSetControl|{this.Name}.fieldControl|']");
        }
    }
}