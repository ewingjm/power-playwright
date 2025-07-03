namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Text.Json.Serialization;
    using PowerPlaywright.Framework.Redirectors;

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

        /// <inheritdoc/>
        [JsonIgnore]
        public int ReleaseChannel => (int)this.ReleaseChannelEnum;
    }
}