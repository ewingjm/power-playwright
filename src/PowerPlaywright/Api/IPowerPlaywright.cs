namespace PowerPlaywright.Api
{
    using System;
    using System.Threading.Tasks;
    using global::PowerPlaywright.Framework.Pages;
    using Microsoft.Playwright;

    /// <summary>
    /// The entrypoint for Power Playwright.
    /// </summary>
    public interface IPowerPlaywright
    {
        /// <summary>
        /// Launches a model-driven app.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="environmentUrl">The environment URL.</param>
        /// <param name="uniqueName">The unique name of the app.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The app.</returns>
        Task<IModelDrivenAppPage> LaunchAppAsync(IBrowserContext browserContext, Uri environmentUrl, string uniqueName, string username, string password);

        /// <summary>
        /// Launches a model-driven app.
        /// </summary>
        /// <typeparam name="TModelDrivenAppPage">The type of home page.</typeparam>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="environmentUrl">The environment URL.</param>
        /// <param name="uniqueName">The unique name of the app.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The app.</returns>
        Task<TModelDrivenAppPage> LaunchAppAsync<TModelDrivenAppPage>(IBrowserContext browserContext, Uri environmentUrl, string uniqueName, string username, string password)
            where TModelDrivenAppPage : IModelDrivenAppPage;
    }
}