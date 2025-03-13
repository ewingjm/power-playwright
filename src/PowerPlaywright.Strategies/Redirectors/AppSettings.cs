namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// App settings.
    /// </summary>
    internal class AppSettings
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
        public ReleaseChannel AppChannel { get; private set; }
    }
}