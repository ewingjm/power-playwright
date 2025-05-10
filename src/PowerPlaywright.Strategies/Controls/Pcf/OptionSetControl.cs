namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
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

            var flyoutDiv = this.Page.Locator("div[id^='fluent-listbox']").First;

            for (int attempt = 1; attempt <= 10; attempt++)
            {
                if (await flyoutDiv.IsVisibleAsync())
                    break;

                if (attempt == 10)
                    throw new PowerPlaywrightException("Unable to locate the optionset flyout after 10 attempts.");

                await toggleMenu.ClickIfVisibleAsync(hoverOver: true, scrollIntoView: true);
            }

            //Gets the last one as the flyout is at the end of the dom
            var option = this.Page.GetByRole(AriaRole.Option, new PageGetByRoleOptions { Name = optionValue }).Last;

            await option.ScrollIntoViewIfNeededAsync();
            await option.HoverAsync();

            await option.ClickIfVisibleAsync(true, true);
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-lp-id*='PowerApps.CoreControls.OptionSetControl|{this.Name}.fieldControl|']");
        }
    }
}