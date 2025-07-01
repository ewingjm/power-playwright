namespace PowerPlaywright.Strategies.Redirectors
{
    using PowerPlaywright.Framework.Redirectors;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The user settings.
    /// </summary>
    internal class UserSettings : IUserSettings
    {
        [JsonInclude]
        [JsonPropertyName("appToggles")]
        public AppToggles AppTogglesTyped { get; private set; }

        [JsonInclude]
        [JsonPropertyName("releaseChannel")]
        public ReleaseChannelOverride ReleaseChannelEnum { get; private set; }

        /// <inheritdoc />
        [JsonIgnore]
        public int ReleaseChannel => (int)ReleaseChannelEnum;

        /// <inheritdoc />
        [JsonIgnore]
        public IDictionary<string, bool?> AppToggles => this.AppTogglesTyped.ToDictionary();
    }
}
