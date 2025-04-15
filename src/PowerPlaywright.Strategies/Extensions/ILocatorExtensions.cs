namespace PowerPlaywright.Strategies.Extensions
{
    using Microsoft.Playwright;
    using System;
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

            return string.IsNullOrEmpty(value) || value == PowerAppsInputPlaceholderValue ? null : value;
        }

        /// <summary>
        /// Returns the value returned by <see cref="ILocator.InputValueAsync(LocatorInputValueOptions)"/> but transforms placeholder values (e.g. '---') to null.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        internal static async Task<T> InputValueOrNullAsync<T>(this ILocator locator, LocatorInputValueOptions options = null)
        {
            var value = await locator.InputValueAsync(options);

            return string.IsNullOrEmpty(value) || value == PowerAppsInputPlaceholderValue
                ? default
                : ConvertTo<T>(value);
        }

        private static T ConvertTo<T>(object value)
        {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return value == null
                ? default
                : (T)Convert.ChangeType(value, targetType);
        }
    }
}