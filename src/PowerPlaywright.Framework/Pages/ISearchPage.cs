namespace PowerPlaywright.Framework.Pages
{
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Controls.Platform;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// A model-driven app page.
    /// </summary>
    public interface ISearchPage : IAppPage
    {
        /// <summary>
        /// Gets the site map control.
        /// </summary>
        ISiteMap SiteMap { get; }

        /// <summary>
        /// Gets an object that provides client API functionality.
        /// </summary>
        IClientApi ClientApi { get; }

        /// <summary>
        /// Opens a specific search tab on the search results page unless it is already selected.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<ISearchPage> OpenTabAsync(string search);
    }
}