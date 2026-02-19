namespace PowerPlaywright.IntegrationTests.Extensions
{
    using System.Security.Cryptography;
    using System.Text;

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
    }
}
