namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using PowerPlaywright.Framework.Redirectors;

    /// <summary>
    /// The user settings.
    /// </summary>
    internal class UserSettings : IUserSettings
    {
        /// <summary>
        /// Gets the app toggles.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("appToggles")]
        public AppToggles AppTogglesTyped { get; private set; }

        /// <summary>
        /// Gets the release channel.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("releaseChannel")]
        public ReleaseChannelOverride ReleaseChannelEnum { get; private set; }

        /// <inheritdoc />
        [JsonIgnore]
        public int ReleaseChannel => (int)this.ReleaseChannelEnum;

        /// <inheritdoc />
        [JsonIgnore]
        public IDictionary<string, bool?> AppToggles => this.AppTogglesTyped.ToDictionary();
    }
}
