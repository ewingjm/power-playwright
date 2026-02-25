namespace PowerPlaywright.IntegrationTests.Extensions
{
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extensions to the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a strings to a deterministic GUID (i.e. the same string always returns the same GUID).
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The GUID.</returns>
        public static Guid ToDeterministicGuid(this string s)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(s));

            return new Guid(hash.AsSpan(0, 16));
        }

        /// <summary>
        /// Removes HTML tags from a string.
        /// </summary>
        /// <param name="input">The input string containing HTML.</param>
        /// <returns>The string with HTML tags removed.</returns>
        public static string? ToClearTextFromHtml(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
            }

            var text = Regex.Replace(input, @"<[^>]+>", string.Empty);
            text = WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"\s+", string.Empty);

            return string.IsNullOrEmpty(text) ? null : text;
        }
    }
}