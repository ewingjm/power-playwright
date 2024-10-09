namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.Packaging.Signing;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using PowerPlaywright.Extensions;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Extensions;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.Pages;
    using PowerPlaywright.Resolvers;

    /// <summary>
    /// Represents a model-driven app.
    /// </summary>
    public class ModelDrivenApp : IModelDrivenApp, IAsyncDisposable
    {
        private const string StrategiesPackageId = "PowerPlaywright.Strategies";

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);
        private static bool strategiesInstalled;

        private readonly IBrowserContext browserContext;
        private readonly IPageFactory pageFactory;
        private readonly IEnumerable<IAppLoadInitializable> initializeOnLoad;
        private readonly ILogger<ModelDrivenApp> logger;

        private bool loggedIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelDrivenApp"/> class.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="pageFactory">The page factory.</param>
        /// <param name="initializeOnLoad">The objects to initialise on app load.</param>
        /// <param name="logger">The logger.</param>
        private ModelDrivenApp(IBrowserContext browserContext, IPageFactory pageFactory, IEnumerable<IAppLoadInitializable> initializeOnLoad = null, ILogger<ModelDrivenApp> logger = null)
        {
            this.browserContext = browserContext ?? throw new ArgumentNullException(nameof(browserContext));
            this.pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
            this.initializeOnLoad = initializeOnLoad;
            this.logger = logger;
        }

        /// <summary>
        /// Launch a new model-driven app instance.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <returns>The model-driven app instance.</returns>
        public static Task<ModelDrivenApp> LaunchAsync(IBrowserContext browserContext)
        {
            return LaunchInternalAsync(browserContext, "https://api.nuget.org/v3/index.json");
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

            // TODO: Implement cookie based login
            if (currentPage is LoginPage loginPage)
            {
                homePage = await loginPage.LoginAsync(username, password);
            }
            else
            {
                homePage = (IModelDrivenAppPage)currentPage;
            }

            this.loggedIn = true;

            await homePage.Page.GotoAsync(homePage.Page.Url + "&flags=easyreproautomation%3Dtrue%2Ctestmode%3Dtrue");
            await homePage.Page.WaitForAppIdleAsync();

            await Task.WhenAll(this.initializeOnLoad.Select(i => i.InitializeAsync(homePage.Page)));

            return homePage;
        }

        /// <summary>
        /// Launch a new model-driven app instance with the given NuGet source.
        /// </summary>
        /// <param name="browserContext">The browser context.</param>
        /// <param name="nugetSource">The source NuGet feed.</param>
        /// <returns>The model-driven app instance.</returns>
        internal static async Task<ModelDrivenApp> LaunchInternalAsync(IBrowserContext browserContext, string nugetSource)
        {
            var strategiesPackageIdentity = await InstallStrategiesAsync(nugetSource);

            return ConfigureServices(browserContext, strategiesPackageIdentity)
                .GetRequiredService<ModelDrivenApp>();
        }

        private static async Task<PackageIdentity> InstallStrategiesAsync(string source)
        {
            var cache = new SourceCacheContext();
            var repository = Repository.Factory.GetCoreV3(source);

            var findPackageByIdResource = await repository.GetResourceAsync<FindPackageByIdResource>();

            var version = await FindBestStrategiesVersionAsync(cache, findPackageByIdResource);
            var identity = new PackageIdentity(StrategiesPackageId, version);

            await Semaphore.WaitAsync();
            try
            {
                if (!strategiesInstalled)
                {
                    using (var packageStream = new MemoryStream())
                    {
                        await CopyStrategiesToStreamAsync(cache, findPackageByIdResource, version, packageStream);
                        await AddStragiesToGlobalPackagesFolderAsync(source, identity, packageStream);
                    }

                    strategiesInstalled = true;
                }
            }
            finally
            {
                Semaphore.Release();
            }

            return identity;
        }

        private static async Task<NuGetVersion> FindBestStrategiesVersionAsync(SourceCacheContext cache, FindPackageByIdResource findPackageByIdResource)
        {
            var versions = await findPackageByIdResource.GetAllVersionsAsync(
                StrategiesPackageId,
                cache,
                NullLogger.Instance,
                CancellationToken.None);

            var majorVersion = Assembly.GetExecutingAssembly().GetName().Version.Major;

            return versions.Where(v => v.Major == majorVersion).FindBestMatch(VersionRange.Parse($"{majorVersion}.*"), v => v)
                ?? throw new PowerPlaywrightException($"Unable to find strategies version matching '{majorVersion}.*'.");
        }

        private static async Task CopyStrategiesToStreamAsync(SourceCacheContext cache, FindPackageByIdResource findPackageByIdResource, NuGetVersion version, Stream packageStream)
        {
            var copiedToStream = await findPackageByIdResource.CopyNupkgToStreamAsync(
                StrategiesPackageId,
                version,
                packageStream,
                cache,
                NullLogger.Instance,
                CancellationToken.None);

            if (!copiedToStream)
            {
                throw new PowerPlaywrightException($"Failed to download version {version} of the {StrategiesPackageId} package.");
            }

            packageStream.Seek(0, SeekOrigin.Begin);
        }

        private static async Task<PackageIdentity> AddStragiesToGlobalPackagesFolderAsync(string source, PackageIdentity identity, MemoryStream packageStream)
        {
            var settings = Settings.LoadDefaultSettings(null);

            await GlobalPackagesFolderUtility.AddPackageAsync(
                source,
                identity,
                packageStream,
                SettingsUtility.GetGlobalPackagesFolder(settings),
                parentId: Guid.Empty,
                ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                NullLogger.Instance,
                CancellationToken.None);

            return identity;
        }

        private static ServiceProvider ConfigureServices(IBrowserContext browserContext, PackageIdentity strategiesPackageIdentity)
        {
            return new ServiceCollection()
                .AddLogging()
                .AddSingleton(sp => new ModelDrivenApp(browserContext, sp.GetRequiredService<IPageFactory>(), sp.GetServices<IAppLoadInitializable>(), sp.GetService<ILogger<ModelDrivenApp>>()))
                .AddSingleton<IAssemblyProvider, GlobalPackagesAssemblyProvider>(sp => new GlobalPackagesAssemblyProvider(strategiesPackageIdentity, sp.GetRequiredService<ILogger<GlobalPackagesAssemblyProvider>>()))
                .AddAppLoadInitializedSingleton<IControlFactory, ControlFactory>()
                .AddSingleton<IPageFactory, PageFactory>()
                .AddSingleton<IControlStrategyResolver, ExternalControlStrategyResolver>()
                .AddAppLoadInitializedSingleton<IControlStrategyResolver, PcfControlStrategyResolver>()
                .AddAppLoadInitializedSingleton<IControlStrategyResolver, PlatformControlStrategyResolver>()
                .BuildServiceProvider();
        }
    }
}