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

            try
            {
                await page.WaitForDomStableAsync(TimeSpan.FromMilliseconds(500));
                await page.WaitForFunctionAsync("typeof window.UCWorkBlockTracker?.isAppIdle === 'function' && window.UCWorkBlockTracker.isAppIdle() === true", options: new PageWaitForFunctionOptions { Timeout = (int)timeout.TotalMilliseconds });
            }
            catch (TimeoutException)
            {
                // App still not idle after timeout which appears to occasionally happens to due certain assets not being loaded - swallow and attempt to continue.
            }

            await WaitForSaveAsync(page, timeout);
        }

        /// <summary>
        /// Waits until no DOM mutations have occurred for the given quiet period.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="quietPeriod">The period without DOM changes required for the DOM to be considered stable.</param>
        /// <param name="timeout">The maximum time to wait. Defaults to 30 seconds.</param>
        /// <returns>A <see cref="Task"/> that resolves to true if the DOM became stable, or false if the timeout elapsed first.</returns>
        public static async Task<bool> WaitForDomStableAsync(this IPage page, TimeSpan quietPeriod, TimeSpan timeout = default)
        {
            if (timeout == default)
            {
                timeout = TimeSpan.FromSeconds(30);
            }

            const string script = @"({ quiet, timeout }) => new Promise(resolve => {
                let quietTimer;
                const finish = stable => {
                    observer.disconnect();
                    clearTimeout(quietTimer);
                    clearTimeout(maxTimer);
                    resolve(stable);
                };
                const arm = () => {
                    clearTimeout(quietTimer);
                    quietTimer = setTimeout(() => finish(true), quiet);
                };
                const observer = new MutationObserver(arm);
                observer.observe(document, { childList: true, subtree: true, attributes: true, characterData: true });
                const maxTimer = setTimeout(() => finish(false), timeout);
                arm();
            })";

            return await page.EvaluateAsync<bool>(script, new { quiet = (int)quietPeriod.TotalMilliseconds, timeout = (int)timeout.TotalMilliseconds });
        }

        private static async Task WaitForSaveAsync(IPage page, TimeSpan timeout)
        {
            var savingAlert = page.GetByRole(AriaRole.Alert).Filter(new LocatorFilterOptions { HasText = "Saving..." }).Or(page.GetByRole(AriaRole.Progressbar));

            if (await savingAlert.IsVisibleAsync())
            {
                await savingAlert.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = (int)timeout.TotalMilliseconds });
            }
        }
    }
}