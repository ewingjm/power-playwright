namespace PowerPlaywright.TestApp.Model.Search
{
    using System.Collections.Generic;

    /// <summary>
    /// Class representing the search query response.
    /// </summary>
    public class SearchQueryResponse
    {
        /// <summary>
        /// Gets or sets error.
        /// </summary>
        public object? Error { get; set; }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public List<SearchResultItem>? Value { get; set; }

        /// <summary>
        /// Gets or sets facets.
        /// </summary>
        public Dictionary<string, object>? Facets { get; set; }

        /// <summary>
        /// Gets or sets query context.
        /// </summary>
        public QueryContext? QueryContext { get; set; }

        /// <summary>
        /// Gets or sets count.
        /// </summary>
        public int Count { get; set; }
    }
}