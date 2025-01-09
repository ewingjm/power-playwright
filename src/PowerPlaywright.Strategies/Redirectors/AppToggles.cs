namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// User app toggles.
    /// </summary>
    internal class AppToggles
    {
        /// <summary>
        /// Gets a value indicating whether modernizationOptOut is toggled.
        /// </summary>
        [JsonInclude]
        public bool? ModernizationOptOut { get; private set; }
    }
}