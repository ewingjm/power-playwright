namespace PowerPlaywright.IntegrationTests.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extensions for the <see cref="Enum"/> class.
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Converts an enum to a spaced string representation.
        /// </summary>
        /// <param name="enum">The enum.</param>
        /// <returns>The spaced string.</returns>
        internal static string ToDisplayName(this Enum @enum)
        {
            return Regex.Replace(@enum.ToString(), "(?<=[a-z])([A-Z])", " $1");
        }
    }
}
