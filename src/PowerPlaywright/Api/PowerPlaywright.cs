namespace PowerPlaywright.Api
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using global::PowerPlaywright.Config;
    using global::PowerPlaywright.Extensions;
    using global::PowerPlaywright.Framework;
    using global::PowerPlaywright.Framework.Pages;
    using global::PowerPlaywright.Resolvers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Playwright;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;

    /// <summary>
    /// The entry point for Power Playwright.
    /// </summary>
    public class PowerPlaywright : IPowerPlaywright
    {
        private const string StrategiesPackageId = "PowerPlaywright.Strategies";
        private const string NuGetV3Source = "https://api.nuget.org/v3/index.json";
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private static PackageIdentity installedStrategiesPackage;
        private static INuGetPackageInstaller packageInstaller;

        private readonly PowerPlaywrightConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerPlaywright"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the parameters are null.</exception>
        internal PowerPlaywright(PowerPlaywrightConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Creates an instance of Power Playwright.
        /// </summary>
        /// <returns>A Power Playwright instance.</returns>
        public static async Task<IPowerPlaywright> CreateAsync(PowerPlaywrightConfiguration configuration = null)
        {
            return await CreateInternalAsync(await GetNuGetPackageInstaller(), configuration);
        }

        /// <inheritdoc/>
        public Task<IModelDrivenAppPage> LaunchAppAsync(IBrowserContext browserContext, Uri environmentUrl, string uniqueName, string username, string password)
        {
            if (environmentUrl is null)
            {
                throw new ArgumentNullException(nameof(environmentUrl));
            }

            if (string.IsNullOrEmpty(uniqueName))
            {
                throw new ArgumentException($"'{nameof(uniqueName)}' cannot be null or empty.", nameof(uniqueName));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            return this.LaunchAppInternalAsync<IModelDrivenAppPage>(
                browserContext, environmentUrl, uniqueName, username, password);
        }

        /// <inheritdoc/>
        public Task<TModelDrivenAppPage> LaunchAppAsync<TModelDrivenAppPage>(IBrowserContext browserContext, Uri environmentUrl, string uniqueName, string username, string password)
            where TModelDrivenAppPage : IModelDrivenAppPage
        {
            if (environmentUrl is null)
            {
                throw new ArgumentNullException(nameof(environmentUrl));
            }

            if (string.IsNullOrEmpty(uniqueName))
            {
                throw new ArgumentException($"'{nameof(uniqueName)}' cannot be null or empty.", nameof(uniqueName));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            return this.LaunchAppInternalAsync<TModelDrivenAppPage>(
                browserContext, environmentUrl, uniqueName, username, password);
        }

        /// <summary>
        /// Creates an instance of Power Playwright.
        /// </summary>
        /// <param name="packageInstaller">The NuGet package installer.</param>
        /// <returns>A Power Playwright instance.</returns>
        internal static async Task<IPowerPlaywright> CreateInternalAsync(INuGetPackageInstaller packageInstaller, PowerPlaywrightConfiguration configuration = null)
        {
            if (packageInstaller is null)
            {
                throw new ArgumentNullException(nameof(packageInstaller));
            }

            configuration = configuration ?? new PowerPlaywrightConfiguration();
            configuration.PackageIdentity = installedStrategiesPackage ?? await InstallStrategiesAsync(packageInstaller);

            return new PowerPlaywright(configuration);
        }

        /// <summary>
        /// Launches a model-driven app and logs in.
        /// </summary>
        /// <typeparam name="TModelDrivenAppPage">The type of the model-driven app page to return after login.</typeparam>
        /// <param name="browserContext">The browser context to use.</param>
        /// <param name="environmentUrl">The URL of the environment to launch the app in.</param>
        /// <param name="uniqueName">The unique name of the app to launch.</param>
        /// <param name="username">The username to use to log in to the app.</param>
        /// <param name="password">The password to use to log in to the app.</param>
        /// <returns>A <see cref="Task"/> representing the operation. The task result is the model-driven app page after login.</returns>
        internal Task<TModelDrivenAppPage> LaunchAppInternalAsync<TModelDrivenAppPage>(IBrowserContext browserContext, Uri environmentUrl, string uniqueName, string username, string password)
            where TModelDrivenAppPage : IModelDrivenAppPage
        {
            return this
                .GetModelDrivenApp(browserContext, environmentUrl, uniqueName)
                .LoginAsync<TModelDrivenAppPage>(username, password);
        }

        private static async Task<INuGetPackageInstaller> GetNuGetPackageInstaller()
        {
            if (packageInstaller is null)
            {
                var findPackageByIdResource = await Repository.Factory
                    .GetCoreV3(NuGetV3Source)
                    .GetResourceAsync<FindPackageByIdResource>();

                packageInstaller = new NuGetPackageInstaller(findPackageByIdResource, NuGetV3Source);
            }

            return packageInstaller;
        }

        private static async Task<PackageIdentity> InstallStrategiesAsync(INuGetPackageInstaller packageInstaller)
        {
            await Semaphore.WaitAsync();

            try
            {
                if (installedStrategiesPackage is null)
                {
                    var identity = await FindBestStrategiesPackageVersionAsync(packageInstaller);

                    await packageInstaller.InstallPackageAsync(identity);

                    installedStrategiesPackage = identity;
                }
            }
            finally
            {
                Semaphore.Release();
            }

            return installedStrategiesPackage;
        }

        private static async Task<PackageIdentity> FindBestStrategiesPackageVersionAsync(INuGetPackageInstaller nuGetPackageInstaller)
        {
            var majorVersion = Assembly.GetExecutingAssembly().GetName().Version.Major;

            var versions = await nuGetPackageInstaller.GetAllVersionsAsync(StrategiesPackageId);

            var matchingMajorVersions = versions.Where(v => v.Major == majorVersion);
            var idealVersion = matchingMajorVersions.Count() > 1
                ? matchingMajorVersions.FindBestMatch(VersionRange.Parse($"{majorVersion}.*"), v => v)
                : matchingMajorVersions.FirstOrDefault()
                ?? throw new PowerPlaywrightException($"Unable to find strategies version matching '{majorVersion}.*'.");

            return new PackageIdentity(StrategiesPackageId, idealVersion);
        }

        private IModelDrivenApp GetModelDrivenApp(IBrowserContext browserContext, Uri environmentUrl, string uniqueName)
        {
            return this
                .ConfigureServices(browserContext, environmentUrl, uniqueName)
                .GetService<IModelDrivenApp>();
        }

        private ServiceProvider ConfigureServices(IBrowserContext browserContext, Uri environmentUrl, string appUniqueName)
        {
            return new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .AddControlRedirectionInfoProvider()
                .AddSingleton(browserContext)
                .AddSingleton(this.configuration.PackageIdentity)
                .AddSingleton(sp => Settings.LoadDefaultSettings(null))
                .Configure<ModelDrivenAppOptions>(opts =>
                {
                    opts.EnvironmentUrl = environmentUrl;
                    opts.AppUniqueName = appUniqueName;
                })
                .AddSingleton<IModelDrivenApp, ModelDrivenApp>()
                .AddSingleton<IAssemblyProvider, GlobalPackagesAssemblyProvider>()
                .AddAdditionalControlAssemblies(this.configuration.ControlAssemblies)
                .AddSingleton<IControlFactory, ControlFactory>()
                .AddSingleton<IPageFactory, PageFactory>()
                .AddSingleton<IControlStrategyResolver, ExternalControlStrategyResolver>()
                .AddSingleton<IControlStrategyResolver, PcfControlStrategyResolver>()
                .AddSingleton<IControlStrategyResolver, PlatformControlStrategyResolver>()
                .AddSingleton<IPlatformReference, PlatformReference>()
                .AddAppLoadInitializedSingleton<IEnvironmentInfoProvider, EnvironmentInfoProvider>()
                .BuildServiceProvider();
        }
    }
}