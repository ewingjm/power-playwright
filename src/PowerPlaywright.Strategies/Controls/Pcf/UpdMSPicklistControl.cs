namespace PowerPlaywright.Strategies.Controls.Pcf
{
    using PowerPlaywright.Framework.Controls.Pcf.Attributes;
    using PowerPlaywright.Framework.Controls.Pcf;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Playwright;
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Framework.Controls;
    using System.Linq;
    using PowerPlaywright.Framework.Extensions;

    /// <summary>
    /// A control strategy for the <see cref="IUpdMSPicklistControl"/>.
    /// </summary>
    [PcfControlStrategy(0, 0, 0)]
    public class UpdMSPicklistControl : PcfControl, IUpdMSPicklistControl
    {
        private readonly ILocator toggleMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdMSPicklistControl"/> class.
        /// </summary>
        /// <param name="name">The name given to the control.</param>
        /// <param name="appPage">The app page.</param>
        /// <param name="parent">The parent control.</param>
        public UpdMSPicklistControl(string name, IAppPage appPage, IControl parent = null) : base(name, appPage, parent)
        {
            this.toggleMenu = this.Container.Locator("button[aria-label='Toggle menu']");
        }

        /// <inheritdoc/>
        public async Task<List<int>> GetNumericValuesAsync()
        {
            var liElements = await this.Container.Locator("ul").First.Locator("li").AllAsync();

            if (liElements == null || liElements.Count == 0)
                return new List<int>();

            var dataValues = await Task.WhenAll(
                liElements.Select(async li =>
                {
                    var valueStr = await li.GetAttributeAsync("data-value");
                    return int.TryParse(valueStr, out var number) ? number : (int?)null;
                })
            );

            return dataValues.Where(v => v.HasValue).Select(v => v.Value).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<string>> GetValueAsync()
        {
            var liElements = await this.Container.Locator("ul").First.Locator("li").AllAsync();

            if (liElements == null || liElements.Count == 0)
                return null;

            var dataValues = await Task.WhenAll(
                liElements.Select(async li =>
                {
                    var span = li.Locator("span").First;
                    return await span.InnerTextAsync();
                })
            );

            var result = dataValues.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

            return result.Any() ? result : null;
        }

        /// <inheritdoc/>
        public async Task SelectAllAsync()
        {
            await this.Page.WaitForAppIdleAsync();
            await this.Container.ClickAsync();

            await toggleMenu.HoverAsync();
            await toggleMenu.ScrollIntoViewIfNeededAsync();
            await toggleMenu.ClickAsync();

            var selectAllCheckBox = this.Container.GetByText("Select All");
            await selectAllCheckBox.ClickAsync();
        }

        public Task SetByNumericValueAsync(List<int> optionValues)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task SetValueAsync(List<string> optionValues)
        {
            await this.Page.WaitForAppIdleAsync();
            await this.Container.ClickAsync();

            await toggleMenu.HoverAsync();
            await toggleMenu.ScrollIntoViewIfNeededAsync();
            await toggleMenu.ClickAsync();

            foreach (var optionValue in optionValues)
            {
                var el = this.Container.GetByTitle(optionValue);

                if (await el.IsVisibleAsync())
                {
                    await el.ClickAsync();
                }
            }
        }

        /// <inheritdoc/>
        protected override ILocator GetRoot(ILocator context)
        {
            return context.Locator($"div[data-id*='{this.Name}.fieldControl_container']");
        }
    }
}