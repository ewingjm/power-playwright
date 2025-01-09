namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The user settings.
    /// </summary>
    internal class UserSettings
    {
        /// <summary>
        /// Gets the try toggle sets.
        /// </summary>
        [JsonInclude]
        public AppToggles TryToggleSets { get; private set; }

        /// <summary>
        /// Gets the model app channel override.
        /// </summary>
        [JsonInclude]
        public ReleaseChannelOverride ReleaseChannel { get; private set; }
    }
}
