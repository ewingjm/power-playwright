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
        /// Waits for the DOM and app to remain idle together for 300 milliseconds.
        /// </summary>
        /// <remarks>
        /// Samples the app idle tracker every 50 milliseconds, requiring at least seven consecutive
        /// idle readings with no DOM mutations. A busy reading or unavailable tracker restarts the quiet period.
        /// If the timeout expires, logs a warning and continues to wait for any active save to finish.
        /// </remarks>
        /// <param name="page">The page.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task WaitForAppIdleAsync(this IPage page, TimeSpan timeout = default)
        {
            if (timeout == default)
            {
                timeout = TimeSpan.FromSeconds(30);
            }

            const string script = @"async ({ quiet, interval, minimumSamples, timeout }) => {
                let idleSince = null;
                let samples = 0;
                const reset = () => {
                    idleSince = null;
                    samples = 0;
                };
                const observer = new MutationObserver(reset);
                const deadline = performance.now() + timeout;
                observer.observe(document, { childList: true, subtree: true, attributes: true, characterData: true });
                try {
                    while (performance.now() < deadline) {
                        const tracker = window.UCWorkBlockTracker;
                        if (typeof tracker?.isAppIdle === 'function' && tracker.isAppIdle() === true) {
                            const now = performance.now();
                            if (idleSince === null) {
                                idleSince = now;
                            }
                            samples++;
                            if (samples >= minimumSamples && now - idleSince >= quiet) {
                                if (observer.takeRecords().length === 0) {
                                    return true;
                                }
                                reset();
                            }
                        } else {
                            reset();
                        }
                        await new Promise(resolve => setTimeout(resolve, Math.min(interval, Math.max(0, deadline - performance.now()))));
                    }
                    return false;
                } finally {
                    observer.disconnect();
                }
            }";

            await page.EvaluateAsync<bool>(script, new { quiet = 300, interval = 50, minimumSamples = 7, timeout = timeout.TotalMilliseconds });

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