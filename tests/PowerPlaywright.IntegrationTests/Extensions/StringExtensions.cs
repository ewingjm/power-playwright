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

        /// <summary>
        /// Determines if a text is an HTML string by checking for the presence of HTML tags.
        /// </summary>
        /// <param name="input">The input string to check for HTML tags.</param>
        /// <returns>True if the string contains HTML tags; otherwise, false.</returns>
        public static bool IsHtml(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            return Regex.IsMatch(input, @"</?[a-z][\s\S]*>", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Converts an HTML-formatted string to plain text by removing tags, decoding entities, and normalizing
        /// whitespace.
        /// </summary>
        /// <param name="input">The HTML-formatted string to convert.</param>
        /// <returns>A plain text representation of the input string.</returns>
        /// <exception cref="ArgumentException">Thrown when the input is null or whitespace.</exception>
        public static string ToPlainTextFromHtml(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or whitespace.", nameof(input));
            }

            input = Regex.Replace(input, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            var text = Regex.Replace(input, @"<[^>]+>", string.Empty);

            text = WebUtility.HtmlDecode(text);

            text = Regex.Replace(text, @"\r?\n\s*", "\n").Trim();

            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }
    }
}