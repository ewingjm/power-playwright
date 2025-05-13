namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls;
    using PowerPlaywright.Framework.Controls.Pcf;
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A control strategy for the <see cref="IUpdMSPicklistControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class UpdMSPicklistControl : PcfControlInternal, IUpdMSPicklistControl
    {
        private readonly ILocator toggleMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdMSPicklistControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="infoProvider"></param>
        /// <param name="parent">The parent control.</param>
        public UpdMSPicklistControl(string name, IAppPage appPage, IEnvironmentInfoProvider infoProvider, IControl parent = null) : base(name, appPage, infoProvider, parent)
        {
            this.toggleMenu = this.Container.Locator("button[aria-label='Toggle menu']");
        }

        /// <inheritdoc/>
        public async Task<Dictionary<int, string>> GetValueAsync()
        {
            var liElements = await this.Container.Locator("ul").First.Locator("li").AllAsync();

            if (liElements == null || liElements.Count == 0)
                return null;

            var keyValuePairs = await Task.WhenAll(
                liElements.Select(async li =>
                {
                    var span = li.Locator("span").First;
                    var dataValueStr = await li.GetAttributeAsync("data-value");
                    var text = await span.InnerTextAsync();

                    if (int.TryParse(dataValueStr, out int dataValue) && !string.IsNullOrWhiteSpace(text))
                    {
                        return new KeyValuePair<int, string>(dataValue, text);
                    }

                    return default;
                })
            );

            var result = keyValuePairs
                .Where(kvp => !kvp.Equals(default(KeyValuePair<int, string>)))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return result.Count > 0 ? result : null;
        }

        /// <inheritdoc/>
        public async Task SelectAllAsync()
        {
            await this.Page.WaitForAppIdleAsync();
            await this.Container.ClickAsync();
            await toggleMenu.ClickIfVisibleAsync(hoverOver: true, scrollIntoView: true);

            var selectAllCheckBox = this.Container.GetByText("Select All");
            await selectAllCheckBox.ClickAsync();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(List<string> optionValues)
        {
            await this.Page.WaitForAppIdleAsync();
            await this.Container.ClickAsync();
            await toggleMenu.ClickIfVisibleAsync(hoverOver: true, scrollIntoView: true);

            foreach (var optionValue in optionValues)
            {
                var el = this.Container.GetByTitle(optionValue);
                await el.ClickIfVisibleAsync();
            }
        }
    }
}