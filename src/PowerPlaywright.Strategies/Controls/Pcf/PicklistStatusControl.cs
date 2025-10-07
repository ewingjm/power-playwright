namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using System.Collections.Generic;
    using System.Linq;
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
    /// A control strategy for the <see cref="PicklistStatusControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class PicklistStatusControl : PcfControlInternal, IPicklistStatusControl
    {
        private readonly ILocator button;

        /// <summary>
        /// Initializes a new instance of the <see cref="PicklistStatusControl"/> class.
        /// </summary>
        /// <param name="name">The control name.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider">The info provider.</param>
        /// <param name="parent">The parent control.</param>
        public PicklistStatusControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null)
            : base(name, appPage, infoProvider, parent)
        {
            this.button = this.Container.GetByRole(AriaRole.Button).First;
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync()
        {
            await this.Page.WaitForAppIdleAsync();

            var value = await this.button.TextContentAsync();

            return value.Trim();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(string value)
        {
            await this.button.ClickAndWaitForAppIdleAsync();

            var listBoxId = await this.button.GetAttributeAsync(Attributes.AriaControls);
            var listBox = this.Page.Locator($"[id='{listBoxId}']");
            var listBoxItem = listBox.GetByRole(AriaRole.Option, new LocatorGetByRoleOptions { Name = value, Exact = true });

            await listBoxItem.ClickAndWaitForAppIdleAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetAllValuesAsync()
        {
            await this.button.ClickAndWaitForAppIdleAsync();
            var listBoxId = await this.button.GetAttributeAsync(Attributes.AriaControls);
            if (string.IsNullOrWhiteSpace(listBoxId))
            {
                throw new PowerPlaywrightException("Unable to retrieve the available options because the option set control is not editable.");
            }

            var listBox = this.Page.Locator($"[id='{listBoxId}']");
            var options = listBox.GetByRole(AriaRole.Option);
            var allOptions = await options.AllTextContentsAsync();

            return allOptions
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim());
        }
    }
}
