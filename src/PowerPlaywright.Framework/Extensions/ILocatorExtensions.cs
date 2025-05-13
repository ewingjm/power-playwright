namespace PowerPlaywright.Framework.Extensions
{
    using System.Threading.Tasks;
    using Microsoft.Playwright;

    /// <summary>
    /// Extensions for the <see cref="ILocator"/>.
    /// </summary>
    public static class ILocatorExtensions
    {
        /// <summary>
        /// Clicks the element if it is visible.
        /// </summary>
        /// <param name="locator">The locator.</param>
        /// <param name="hoverOver"></param>
        /// <param name="scrollIntoView"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ClickIfVisibleAsync(this ILocator locator, bool hoverOver = false, bool scrollIntoView = false)
        {
            if (await locator.IsVisibleAsync())
            {
                if (scrollIntoView)
                {
                    await locator.ScrollIntoViewIfNeededAsync();
                }

                if (hoverOver)
                {
                    await locator.HoverAsync();
                }

                await locator.ClickAsync();
            }
            else
            {
                throw new PowerPlaywrightException($"Unable to click the expected element with locator {locator} as it was not visible");
            }
        }
    }
}