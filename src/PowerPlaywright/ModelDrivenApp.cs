namespace PowerPlaywright
{
    using System;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using PowerPlaywright.Controls;
    using PowerPlaywright.Model.Controls;
    using PowerPlaywright.Notifications;
    using PowerPlaywright.Pages;

    /// <summary>
    /// Represents a model-driven app.
    /// </summary>
    public class ModelDrivenApp : IModelDrivenApp
    {
        private readonly IBrowserContext browserContext;
        private readonly IPageFactory pageFactory;
        private readonly IMediator mediator;
        private readonly ILogger<ModelDrivenApp> logger;

        private bool loggedIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenApp"/> class.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="mediator">The event aggregator.</param>
        /// <param name="logger">The logger.</param>
        internal ModelDrivenApp(IBrowserContext browserContext, IPageFactory pageFactory, IMediator mediator, ILogger<ModelDrivenApp> logger = null)
        {
            this.browserContext = browserContext ?? throw new ArgumentNullException(nameof(browserContext));
            this.pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

            if (currentPage is LoginPage loginPage)
            {
                await loginPage.LoginAsync(username, password);
            }

            this.loggedIn = true;

            var appPage = await this.pageFactory.CreateInstanceAsync(page);

            await this.mediator.Publish(new AppInitializedNotification(appPage));

            return appPage;
        }

        private static ServiceProvider ConfigureServices(IBrowserContext browserContext)
        {
            return new ServiceCollection()
                .AddLogging()
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(typeof(ModelDrivenApp).Assembly);
                })
                .AddSingleton<ModelDrivenApp, ModelDrivenApp>(sp => new ModelDrivenApp(browserContext, sp.GetRequiredService<IPageFactory>(), sp.GetRequiredService<IMediator>(), sp.GetService<ILogger<ModelDrivenApp>>()))
                .AddSingleton<IControlStrategyAssemblyProvider, LocalControlAssemblyProviders>()
                .AddSingleton<IControlStrategyResolver, PcfControlStrategyResolver>()
                .AddSingleton<IControlStrategyResolver, ExternalControlStrategyResolver>()
                .AddSingleton<IControlFactory, ControlFactory>()
                .AddSingleton<IPageFactory, PageFactory>()
                .BuildServiceProvider();
        }
    }
}