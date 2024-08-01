namespace PowerPlaywright.Model
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// User app toggles.
    /// </summary>
    public class AppToggles
    {
        /// <summary>
        /// Gets a value indicating whether ModernizationOptOut is toggled.
        /// </summary>
        [JsonInclude]
        public bool ModernizationOptOut { get; private set; }
    }
}
