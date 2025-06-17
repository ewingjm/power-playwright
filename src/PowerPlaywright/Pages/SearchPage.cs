namespace PowerPlaywright.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Controls.Platform;
    using PowerPlaywright.Framework.Pages;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// An entity form page.
    /// </summary>
    internal class SearchPage : ModelDrivenAppPage, ISearchPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="controlFactory">The control factory.</param>
        public SearchPage(IPage page, IControlFactory controlFactory)
            : base(page, controlFactory)
        {
        }

        //TODO
        public async Task<ISearchPage> OpenTabAsync(string search)
        {
            var tabHeader = Page.GetByRole(AriaRole.Tablist);
            var tabs = await tabHeader.GetByRole(AriaRole.Tab).AllAsync();

            foreach (var tab in tabs)
            {
                var name = await tab.GetAttributeAsync("name");
                var isSelected = await tab.GetAttributeAsync("aria-selected");
                if (name == search && isSelected == "false")
                {
                    await tab.ClickAsync();
                }
            }

            return null; // Or throw an exception if not found
        }
    }
}