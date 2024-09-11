namespace PowerPlaywright.Framework.Extensions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Playwright;

    /// <summary>
    /// Extensions for the <see cref="IPage"/>.
    /// </summary>
    public static class IPageExtensions
    {
        /// <summary>
        /// Waits for the app to be idle.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task WaitForAppIdleAsync(this IPage page, TimeSpan timeout = default)
        {
            if (timeout == default)
            {
                timeout = TimeSpan.FromSeconds(30);
            }

            await page.WaitForFunctionAsync("window.UCWorkBlockTracker.isAppIdle()", options: new PageWaitForFunctionOptions { Timeout = timeout.Milliseconds });
        }
    }
}
