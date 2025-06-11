namespace PowerPlaywright.Framework.Controls.Platform
{
    using System.Threading.Tasks;
    using PowerPlaywright.Framework.Controls.Platform.Attributes;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// An interface representing global search (relvance search).
    /// </summary>
    [PlatformControl]
    public interface IGlobalSearch : IPlatformControl
    {
        /// <summary>
        /// Performs a relevance search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SearchAsync(string searchText);

        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <typeparam name="TPage">The type of page to open.</typeparam>
        /// <param name="search">The search query to search against.</param>
        /// <param name="index">The index of the result to open.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TPage> OpenSearchResultAsync<TPage>(string search, int index)
            where TPage : IModelDrivenAppPage;

        /// <summary>
        /// Indicates if any results found.
        /// </summary>
        /// <returns></returns>
        Task<bool> HasResultsAsync();
    }
}