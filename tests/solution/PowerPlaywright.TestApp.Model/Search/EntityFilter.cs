namespace PowerPlaywright.TestApp.Model.Search
{
    using Newtonsoft.Json;

    /// <summary>
    /// Maps the entities for the search body.
    /// </summary>
    public class EntityFilter
    {
        /// <summary>
        /// Gets or sets name for the search entites search payload.
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}