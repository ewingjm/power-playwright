namespace PowerPlaywright.TestApp.Model.Search
{
    /// <summary>
    /// Class respresenting the search result query context.
    /// </summary>
    public class QueryContext
    {
        /// <summary>
        /// Gets or sets orginal query.
        /// </summary>
        public string? OriginalQuery { get; set; }

        /// <summary>
        /// Gets or sets altered query.
        /// </summary>
        public string? AlteredQuery { get; set; }

        /// <summary>
        /// Gets or sets reason.
        /// </summary>
        public object? Reason { get; set; }

        /// <summary>
        /// Gets or sets spelling suggestions.
        /// </summary>
        public object? SpellSuggestions { get; set; }
    }
}