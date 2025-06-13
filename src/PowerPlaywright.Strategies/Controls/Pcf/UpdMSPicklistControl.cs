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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IUpdMSPicklistControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class UpdMSPicklistControl : PcfControlInternal, IUpdMSPicklistControl
    {
        private readonly ILocator comboxBox;
        private readonly ILocator buttonToggleMenu;
        private readonly ILocator checkboxSelectAllItems;
        private readonly ILocator textBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdMSPicklistControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider"></param>
        /// <param name="parent">The parent control.</param>
        public UpdMSPicklistControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.comboxBox = this.Container.GetByRole(AriaRole.Combobox);
            this.buttonToggleMenu = this.comboxBox.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Toggle menu" });
            this.checkboxSelectAllItems = this.Container.GetByText("Select all");
            this.textBox = this.Container.GetByRole(AriaRole.Textbox);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var label = await this.textBox.GetLabelAsync();

            var match = Regex.Match(label, "Selected:(.+)");
            if (!match.Success)
            {
                return null;
            }

            return match.Groups[1].Value.Split(',').Select(c => c.Substring(1));
        }

        /// <inheritdoc/>
        public async Task SelectAllAsync()
        {
            await this.ShowListBoxAsync();
            await this.checkboxSelectAllItems.FocusAsync();
            await this.checkboxSelectAllItems.CheckAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(IEnumerable<string> optionValues)
        {
            await this.ShowListBoxAsync();
            await this.ClearExistingOptions();

            foreach (var optionValue in optionValues)
            {
                await this.SelectOptionAsync(optionValue);
            }

            await this.Parent.Container.ClickAndWaitForAppIdleAsync();
        }

        private async Task ClearExistingOptions()
        {
            var selectedOptions = this.Container.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Selected = true });

            foreach (var selectedOption in (await selectedOptions.AllAsync()).Reverse())
            {
                await selectedOption.ClickAndWaitForAppIdleAsync();
            }
        }

        private async Task SelectOptionAsync(string optionValue)
        {
            await this.Container
                .GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Name = optionValue })
                .ClickAndWaitForAppIdleAsync();
        }

        private async Task ShowListBoxAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            if (await this.comboxBox.IsVisibleAsync() && await this.comboxBox.IsExpandedAsync())
            {
                // Already shown
                return;
            }

            if (await this.textBox.IsVisibleAsync())
            {
                // No values set.
                await this.textBox.FocusAsync();
            }
            else
            {
                // Values set.
                await this.comboxBox.HoverAsync();
            }

            await this.buttonToggleMenu.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            // The base GetRoot method does not find choices controls before they have a value.
            return base.GetRoot(context).Or(context.Locator($"div[data-id='{this.Name}.fieldControl_container']"));
        }
    }
}