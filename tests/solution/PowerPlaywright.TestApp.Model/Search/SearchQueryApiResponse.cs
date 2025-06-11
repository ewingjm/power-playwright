namespace PowerPlaywright.TestApp.Model.Search
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the top-level Web API response wrapper for a search query.
    /// Contains metadata and a raw inner JSON string.
    /// </summary>
    public class SearchQueryApiResponse
    {
        /// <summary>
        /// Gets or sets oData context URL provided by the Dataverse Web API.
        /// </summary>
        [JsonProperty("@odata.context")]
        public string? ODataContext { get; set; }

        /// <summary>
        /// Gets or sets raw JSON string containing the actual search results.
        /// </summary>
        [JsonProperty("response")]
        public string? RawResponseJson { get; set; }

        /// <summary>
        /// Gets the raw JSON string into a structured object model.
        /// </summary>
        [JsonIgnore]
        public SearchQueryResponse ParsedResponse =>
            JsonConvert.DeserializeObject<SearchQueryResponse>(this.RawResponseJson);
    }
}