namespace PowerPlaywright
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Assemblies;
    using PowerPlaywright.Events;
    using PowerPlaywright.Model;
    using PowerPlaywright.Model.Events;
    using PowerPlaywright.Pages;
    using PowerPlaywright.Resolvers;

    /// <summary>
    /// Represents a model-driven app.
    /// </summary>
    public class ModelDrivenApp : IModelDrivenApp, IAsyncDisposable
    {
        private readonly IBrowserContext browserContext;
        private readonly IPageFactory pageFactory;
        private readonly IEventAggregator eventAggregator;
        private readonly ILogger<ModelDrivenApp> logger;

        private bool loggedIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenApp"/> class.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="mediator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        internal ModelDrivenApp(IBrowserContext browserContext, IPageFactory pageFactory, IEventAggregator mediator, ILogger<ModelDrivenApp> logger = null)
        {
            this.browserContext = browserContext ?? throw new ArgumentNullException(nameof(browserContext));
            this.pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
            this.eventAggregator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.logger = logger;
        }

        /// <summary>
        /// Launch a new model-driven app instance.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <returns>The model-driven app instance.</returns>
        public static ModelDrivenApp Launch(IBrowserContext browserContext)
        {
            return ConfigureServices(browserContext).GetRequiredService<ModelDrivenApp>();
        }

        /// <summary>
        /// Asynchronously disposes the browser context associated with this model-driven app.
        /// </summary>
        /// <returns>The asynchronous task.</returns>
        public ValueTask DisposeAsync()
        {
            return this.browserContext.DisposeAsync();
        }

        /// <summary>
        /// Logs in to the app.
        /// </summary>
        /// <typeparam name="TModelDrivenAppPage">The expected home page type.</typeparam>
        /// <param name="environmentUrl">The environment URL.</param>
        /// <param name="appUniqueName">The unique name of the app.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<TModelDrivenAppPage> LoginAsync<TModelDrivenAppPage>(Uri environmentUrl, string appUniqueName, string username, string password)
            where TModelDrivenAppPage : IModelDrivenAppPage
        {
            var page = await this.LoginAsync(environmentUrl, appUniqueName, username, password);

            if (page is TModelDrivenAppPage p)
            {
                return p;
            }

            throw new PowerPlaywrightException($"Expected home page to be of type {typeof(TModelDrivenAppPage).Name} but found {page.GetType().Name}.");
        }

        /// <summary>
        /// Logs in to the app.
        /// </summary>
        /// <param name="environmentUrl">The environment URL.</param>
        /// <param name="appUniqueName">The unique name of the app.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IModelDrivenAppPage> LoginAsync(Uri environmentUrl, string appUniqueName, string username, string password)
        {
            if (environmentUrl is null)
            {
                throw new ArgumentNullException(nameof(environmentUrl));
            }

            if (string.IsNullOrEmpty(appUniqueName))
            {
                throw new ArgumentException($"'{nameof(appUniqueName)}' cannot be null or empty.", nameof(appUniqueName));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            if (this.loggedIn)
            {
                throw new PowerPlaywrightException("Already logged in. Please log out before logging in.");
            }

            var page = await this.browserContext.NewPageAsync();
            await page.GotoAsync(new Uri(environmentUrl, $"Apps/uniquename/{appUniqueName}/main.aspx").ToString());

            var currentPage = await this.pageFactory
                .CreateInstanceAsync(page);

            IModelDrivenAppPage homePage;

            if (currentPage is LoginPage loginPage)
            {
                homePage = await loginPage.LoginAsync(username, password);
            }
            else
            {
                homePage = await this.pageFactory.CreateInstanceAsync(page);
            }

            this.loggedIn = true;

            await this.eventAggregator.PublishAsync(new AppInitializedEvent(homePage));

            return homePage;
        }

        private static ServiceProvider ConfigureServices(IBrowserContext browserContext)
        {
            return new ServiceCollection()
                .AddLogging()
                .AddSingleton<ModelDrivenApp, ModelDrivenApp>(sp => new ModelDrivenApp(browserContext, sp.GetRequiredService<IPageFactory>(), sp.GetRequiredService<IEventAggregator>(), sp.GetService<ILogger<ModelDrivenApp>>()))
                .AddSingleton<IEventAggregator, EventAggregator>()
                .AddSingleton<IControlStrategyAssemblyProvider, LocalControlAssemblyProviders>()
                .AddSingleton<IControlStrategyResolver, PcfControlStrategyResolver>()
                .AddSingleton<IControlStrategyResolver, ExternalControlStrategyResolver>()
                .AddSingleton<IControlStrategyResolver, PlatformControlStrategyResolver>()
                .AddSingleton<IControlFactory, ControlFactory>()
                .AddSingleton<IPageFactory, PageFactory>()
                .BuildServiceProvider();
        }
    }
}