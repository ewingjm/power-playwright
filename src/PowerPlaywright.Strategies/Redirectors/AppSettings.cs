namespace PowerPlaywright.Strategies.Redirectors
{
    using PowerPlaywright.Framework.Redirectors;
    using System.Text.Json.Serialization;

    /// <summary>
    /// App settings.
    /// </summary>
    internal class AppSettings : IAppSettings
    {
        /// <summary>
        /// Gets a value indicating whether the NewLookOptOut app setting is enabled.
        /// </summary>
        [JsonInclude]
        public bool NewLookOptOut { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the NewLookOptOut app setting is enabled.
        /// </summary>
        [JsonInclude]
        public bool NewLookAlwaysOn { get; private set; }

        /// <summary>
        /// Gets the app release channel.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("appChannel")]
        public ReleaseChannel AppChannelEnum { get; private set; }

        /// <inheritdoc />
        [JsonIgnore]
        public int AppChannel => (int)this.AppChannelEnum;
    }
}
