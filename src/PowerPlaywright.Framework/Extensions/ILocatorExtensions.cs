namespace PowerPlaywright.Framework.Extensions
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;

    /// <summary>
    /// Extensions for the <see cref="ILocatorExtensions"/>.
    /// </summary>
    public static class ILocatorExtensions
    {
        /// <summary>
        /// Checks if the given locator points to at least one existing element.
        /// </summary>
        /// <param name="locator">The elemnent locator.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task<bool> ElementExistsAsync(this ILocator locator)
        {
            var count = await locator.CountAsync();
            return count > 0;
        }
    }
}