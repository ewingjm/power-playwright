namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Playwright;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;

    /// <summary>
    /// Represents a model-driven app.
    /// </summary>
    internal class ModelDrivenApp : IModelDrivenApp
    {
        private readonly ModelDrivenAppOptions options;
        private readonly IBrowserContext browserContext;
        private readonly IPageFactory pageFactory;
        private readonly IEnumerable<IAppLoadInitializable> initializables;
        private readonly ILogger<ModelDrivenApp> logger;

        private string loggedInUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenApp"/> class.
        /// </summary>
        /// <param name="options">the options.</param>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="initializables">The objects to initialise on app load.</param>
        public ModelDrivenApp(
            IOptions<ModelDrivenAppOptions> options,
            IBrowserContext browserContext,
            IPageFactory pageFactory,
            ILogger<ModelDrivenApp> logger,
            IEnumerable<IAppLoadInitializable> initializables = null)
        {
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.browserContext = browserContext ?? throw new ArgumentNullException(nameof(browserContext));
            this.pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.initializables = initializables;
        }

        /// <summary>
        /// Logs in to the app.
        /// </summary>
        /// <typeparam name="TModelDrivenAppPage">The expected home page type.</typeparam>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="totpSecret">The secret if TOTP is configured.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<TModelDrivenAppPage> LoginAsync<TModelDrivenAppPage>(string username, string password, string totpSecret = null)
            where TModelDrivenAppPage : IModelDrivenAppPage
        {
            var page = await this.LoginAsync(username, password, totpSecret);

            if (page is TModelDrivenAppPage p)
            {
                return p;
            }

            throw new PowerPlaywrightException($"Expected home page to be of type {typeof(TModelDrivenAppPage).Name} but found {page.GetType().Name}.");
        }

        /// <summary>
        /// Logs in to the app.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="totpSecret">The secret if TOTP is configured.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IModelDrivenAppPage> LoginAsync(string username, string password, string totpSecret = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            if (this.loggedInUser != null && !this.loggedInUser.Equals(username, StringComparison.OrdinalIgnoreCase))
            {
                throw new PowerPlaywrightException($"This model-driven app instance is already logged in as {username}. Please create a new instance with a different context to login as a different user.");
            }

            var page = await this.browserContext.NewPageAsync();
            await page.GotoAsync(new Uri(this.options.EnvironmentUrl, $"Apps/uniquename/{this.options.AppUniqueName}").ToString());

            var currentPage = await this.pageFactory.CreateInstanceAsync(page);

            IModelDrivenAppPage homePage;

            // TODO: Implement cookie based login
            if (currentPage is ILoginPage loginPage)
            {
                homePage = await loginPage.LoginAsync(username, password, totpSecret);
            }
            else
            {
                homePage = (IModelDrivenAppPage)currentPage;
            }

            this.loggedInUser = username;

            await homePage.Page.GotoAsync(homePage.Page.Url + "&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue");
            await homePage.Page.WaitForAppIdleAsync();

            await Task.WhenAll(this.initializables.Select(i => i.InitializeAsync(homePage.Page)));

            return homePage;
        }
    }
}