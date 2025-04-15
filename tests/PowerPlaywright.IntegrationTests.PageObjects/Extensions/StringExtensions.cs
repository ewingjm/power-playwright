namespace PowerPlaywright.TestApp.PageObjects.Extensions
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extensions for the <see cref="string"/> type.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Trims any numbers from the end of a string.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The string with numbers removed from the end.</returns>
        internal static string TrimEndNumbers(this string value)
        {
            return Regex.Replace(value, @"\d+$", string.Empty);
        }
    }
}
