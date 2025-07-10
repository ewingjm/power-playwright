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
        /// Enters the search text within relevance search.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="performSearch">A flag to actually perform the search by hitting Enter.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SuggestAsync(string searchText, bool performSearch = false);

        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <typeparam name="TPage">The type of page to open.</typeparam>
        /// <param name="searchText">The search query to search against.</param>
        /// <param name="index">The index of the result to open.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TPage> OpenSuggestionAsync<TPage>(string searchText, int index)
            where TPage : IAppPage;

        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <typeparam name="TPage">The type of page to open.</typeparam>
        /// <param name="search">The search query to search against.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TPage> SearchAsync<TPage>(string search)
            where TPage : IAppPage;

        /// <summary>
        /// Indicates if any results found.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> HasSuggestedResultsAsync();

        /// <summary>
        /// Opens a specific search tab on the search results page unless it is already selected.
        /// </summary>
        /// <param name="searchTabLabel">The </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<ISearchPage> OpenSearchTabAsync(string searchTabLabel);

        /// <summary>
        /// Opens the search result within the selected tab by its index.
        /// </summary>
        /// <param name="index">The index of the list to open.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEntityRecordPage> OpenSearchTabResult(int index);
    }
}