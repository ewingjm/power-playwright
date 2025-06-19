namespace PowerPlaywright.Strategies.Redirectors
{
    using PowerPlaywright.Framework.Redirectors;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The organization settings.
    /// </summary>
    public class OrgSettings : IOrgSettings
    {
        /// <summary>
        /// Gets the release channel.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("releaseChannel")]
        public ReleaseChannel ReleaseChannelEnum { get; private set; }

        /// <summary>
        /// Gets the release channel.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("isexternalsearchindexenabled")]
        public bool IsRelevanceSearchEnabled { get; private set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public int ReleaseChannel => (int)ReleaseChannelEnum;
    }
}