namespace PowerPlaywright.Strategies.Redirectors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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

        /// <summary>
        /// Converts the app toggles to a dictioanry.
        /// </summary>
        /// <returns>A dictionary of app toggles.</returns>
        public IDictionary<string, bool?> ToDictionary()
        {
            return typeof(AppToggles)
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(bool?))
                .ToDictionary(propInfo => propInfo.Name, propInfo => (bool?)propInfo.GetValue(this, null));
        }
    }
}