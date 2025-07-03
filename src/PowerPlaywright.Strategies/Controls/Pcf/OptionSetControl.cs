namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf.Classes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Strategies.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IOptionSetControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class OptionSetControl : PcfControlInternal, IOptionSetControl
    {
        private readonly ILocator toggleMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSetControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The environment info provider.</param>
        /// <param name="parent">The parent control.</param>
        public OptionSetControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.toggleMenu = this.Container.GetByRole(AriaRole.Combobox);
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            return await this.toggleMenu.GetAttributeAsync("value") ?? null;
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string optionValue)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.toggleMenu.ClickAndWaitForAppIdleAsync();
            var option = await this.GetOptionLocatorAsync(optionValue);

            await option.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        async Task IYesNo.SetValueAsync(bool value)
        {
            await this.Page.WaitForAppIdleAsync();

            await this.toggleMenu.ClickAndWaitForAppIdleAsync();

            // TODO: Use table metadata to get true/false text values instead of relying on index.
            var option = await this.GetOptionLocatorAsync(Convert.ToInt32(value));

            await option.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        async Task<bool> IYesNo.GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var value = await this.toggleMenu.GetAttributeAsync("value");
            await this.toggleMenu.ClickAndWaitForAppIdleAsync();

            // TODO: Use table metadata to get true/false text values instead of relying on index.
            var falseOption = await this.GetOptionLocatorAsync(0);
            var falseOptionText = await falseOption.TextContentAsync();

            return value != falseOptionText;
        }

        private async Task<ILocator> GetFlyoutLocatorAsync()
        {
            var flyoutId = await this.toggleMenu.GetAttributeAsync(Attributes.AriaControls);

            return this.Page.GetByRole(AriaRole.Listbox).And(this.Page.Locator($"#{flyoutId}"));
        }

        private async Task<ILocator> GetOptionLocatorAsync(string optionValue)
        {
            var flyout = await this.GetFlyoutLocatorAsync();

            return flyout.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Name = optionValue });
        }

        private async Task<ILocator> GetOptionLocatorAsync(int optionIndex)
        {
            var flyout = await this.GetFlyoutLocatorAsync();

            return flyout.GetByRole(AriaRole.Option).Nth(optionIndex);
        }
    }
}