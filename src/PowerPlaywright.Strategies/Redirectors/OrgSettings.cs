namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The organization settings.
    /// </summary>
    internal class OrgSettings
    {
        /// <summary>
        /// Gets the release channel.
        /// </summary>
        [JsonInclude]
        public ReleaseChannel ReleaseChannel { get; private set; }
    }
}