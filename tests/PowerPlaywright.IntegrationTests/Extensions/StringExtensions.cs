namespace PowerPlaywright.IntegrationTests.Extensions
{
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using PowerPlaywright.TestApp.Model.Search;

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
        /// Builds a search json object from a string array <see cref="string"/> instance.
        /// </summary>
        /// <param name="items">The <see cref="string"/> instance.</param>
        /// <returns>A new <see cref="DateTime"/> instance without the ticks.</returns>
        public static string ToSearchArray(this IEnumerable<string> items)
        {
            var entityList = items?
                .Select(item => new EntityFilter { Name = item })
                .ToList() ?? [];

            return JsonSerializer.Serialize(entityList, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
