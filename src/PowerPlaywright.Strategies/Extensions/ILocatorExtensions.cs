namespace PowerPlaywright.Strategies.Extensions
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework.Extensions;

    /// <summary>
    /// Extensions to the <see cref="ILocator"/> interface.
    /// </summary>
    internal static class ILocatorExtensions
    {
        private const string PowerAppsInputPlaceholderValue = "---";

        /// <summary>
        /// Clicks the locator and waits for the application to become idle.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="timeout">THe timeout value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static async Task ClickAndWaitForAppIdleAsync(this ILocator locator, int timeout = 30000)
        {
            await locator.ClickAsync(new LocatorClickOptions { Timeout = timeout });
            await locator.Page.WaitForAppIdleAsync();
        }

        /// <summary>
        /// Returns the value returned by <see cref="ILocator.InputValueAsync(LocatorInputValueOptions)"/> but transforms placeholder values (e.g. '---') to null.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static async Task<string> InputValueOrNullAsync(this ILocator locator, LocatorInputValueOptions options = null)
        {
            var value = await locator.InputValueAsync(options);

            return string.IsNullOrEmpty(value) || value == PowerAppsInputPlaceholderValue ? null : value;
        }

        /// <summary>
        /// Returns the value returned by <see cref="ILocator.TextContentAsync(LocatorTextContentOptions)"/> but transforms placeholder values (e.g. '---') to null.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static async Task<string> TextContentOrNullAsync(this ILocator locator, LocatorTextContentOptions options = null)
        {
            var value = await locator.TextContentAsync(options);

            return string.IsNullOrEmpty(value) || value == PowerAppsInputPlaceholderValue ? null : value;
        }

        /// <summary>
        /// Returns the value returned by <see cref="ILocator.InputValueAsync(LocatorInputValueOptions)"/> but transforms placeholder values (e.g. '---') to null.
        /// </summary>
        /// <typeparam name="T">The type of value.</typeparam>
        /// <param name="locator">The locator.</param>
        /// <param name="options">The options.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static async Task<T> InputValueOrNullAsync<T>(this ILocator locator, LocatorInputValueOptions options = null)
        {
            var value = await locator.InputValueAsync(options);

            return string.IsNullOrEmpty(value) || value == PowerAppsInputPlaceholderValue
                ? default
                : ConvertTo<T>(value);
        }

        /// <summary>
        /// Gets the value of the 'aria-expanded' attribute. Throws a <see cref="TimeoutException"/> if the attribute is not present.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <returns>Whether the element is expanded.</returns>
        internal static async Task<bool> IsExpandedAsync(this ILocator locator)
        {
            var expanded = await locator.GetAttributeAsync(Attributes.AriaExpanded);

            return bool.Parse(expanded);
        }

        /// <summary>
        /// Gets the value of the 'aria-label' attribute. Throws a <see cref="TimeoutException"/> if the attribute is not present.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <returns>The label.</returns>
        internal static async Task<string> GetLabelAsync(this ILocator locator)
        {
            return await locator.GetAttributeAsync(Attributes.AriaLabel);
        }

        private static T ConvertTo<T>(object value)
        {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return value == null
                ? default
                : (T)Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
        }
    }
}