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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SuggestAsync(string searchText);

        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <param name="searchText">The search query to search against.</param>
        /// <param name="index">The index of the result to open.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEntityRecordPage> OpenSuggestionAsync(string searchText, int index);

        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <typeparam name="TPage">The type of page to open.</typeparam>
        /// <param name="search">The search query to search against.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<TPage> SearchAsync<TPage>(string search)
            where TPage : IAppPage;

        /// <summary>
        /// Opens a page.
        /// </summary>
        /// <param name="search">The search query to search against.</param>
        /// <param name="table">The display name of the table e.g. Accounts, Contacts.</param>
        /// <param name="index">The index of the record to Open.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<IEntityRecordPage> SearchAndOpenResultAsync(string search, string table, int index);

        /// <summary>
        /// Indicates if any results found.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<bool> HasSuggestedResultsAsync();
    }
}