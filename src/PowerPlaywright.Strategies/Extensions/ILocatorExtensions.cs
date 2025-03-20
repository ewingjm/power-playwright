namespace PowerPlaywright.Strategies.Extensions
{
    using Microsoft.Playwright;
    using System.Threading.Tasks;

    internal static class ILocatorExtensions
    {
        private const string PowerAppsInputPlaceholderValue = "---";

        /// <summary>
        /// Returns the value returned by <see cref="ILocator.InputValueAsync(LocatorInputValueOptions)"/> but transforms placeholder values (e.g. '---') to null.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        internal static async Task<string> InputValueOrNullAsync(this ILocator locator, LocatorInputValueOptions options = null)
        {
            var value = await locator.InputValueAsync(options);

            return value != PowerAppsInputPlaceholderValue ? value : null;
        }
    }
}
