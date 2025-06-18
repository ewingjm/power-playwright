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
        /// Opens a search tab from the results across all tables and matches.
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        Task<ISearchPage> OpenTabAsync(string tabName);

        /// <summary>
        /// Opens a record for the given index within the current search tab page.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<IEntityRecordPage> OpenResultAsync(int index);
    }
}