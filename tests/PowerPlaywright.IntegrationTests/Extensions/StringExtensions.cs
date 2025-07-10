namespace PowerPlaywright.IntegrationTests.Extensions
{
    using System.Text.Json;
    using PowerPlaywright.TestApp.Model.Search;

    /// <summary>
    /// Extensions to the <see cref="string"/> class.
    /// </summary>
    internal static class StringExtensions
    {
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