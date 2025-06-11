namespace PowerPlaywright.TestApp.Model.Search
{
    using System.Collections.Generic;

    /// <summary>
    /// Class representing the search results.
    /// </summary>
    public class SearchResultItem
    {
        /// <summary>
        /// Gets or Sets the search result id.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or Sets the search result entity name.
        /// </summary>
        public string? EntityName { get; set; }

        /// <summary>
        /// Gets or Sets the search result objectTypeCode.
        /// </summary>
        public int ObjectTypeCode { get; set; }

        /// <summary>
        /// Gets or Sets the record attributes.
        /// </summary>
        public Attributes? Attributes { get; set; }

        /// <summary>
        /// Gets or Sets the highlight search metrics.
        /// </summary>
        public Dictionary<string, List<string>>? Highlights { get; set; }

        /// <summary>
        /// Gets or Sets the relevance search score.
        /// </summary>
        public double Score { get; set; }
    }
}