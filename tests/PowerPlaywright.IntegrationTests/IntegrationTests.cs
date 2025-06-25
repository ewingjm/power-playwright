namespace PowerPlaywright.IntegrationTests
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Azure.Core;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NUnit.Framework.Interfaces;
    using PowerPlaywright.Api;
    using PowerPlaywright.Config;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.IntegrationTests.Config;
    using PowerPlaywright.TestApp.Model;

    /// <summary>
    /// A base class for integration tests.
    /// </summary>
    public abstract class IntegrationTests : ContextTest
    {
        /// <summary>
        /// The unique name of the app used for testing.
        /// </summary>
        protected const string TestAppUniqueName = "pp_UserInterfaceDemo";

        private const string EnvironmentVariablePrefix = "POWERPLAYWRIGHT:TEST:";

        private static readonly IEnumerator<UserConfiguration> UserEnumerator;

        private bool isTracing;
        private UserConfiguration? user;
        private IPowerPlaywright powerPlaywright;

        static IntegrationTests()
        {
            Configuration = GetConfiguration();

            UserEnumerator = Configuration.Users.GetEnumerator();
        }

        /// <summary>
        /// Gets the test suite configuration.
        /// </summary>
        protected static TestSuiteConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets a user for the current test.
        /// </summary>
        protected UserConfiguration User
        {
            get
            {
                return this.user ??= GetUser();
            }
        }

        /// <summary>
        /// Gets the Power Playwright instance.
        /// </summary>
        protected IPowerPlaywright PowerPlaywright => this.powerPlaywright;

        /// <summary>
        /// Sets up the Power Playwright instance.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task SetUpPowerPlaywrightAsync()
        {
            var packageSource = Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "packages");
            var findPackageByIdResource = await Repository.Factory
                .GetCoreV3(packageSource)
                .GetResourceAsync<FindPackageByIdResource>();

            this.powerPlaywright = await Api.PowerPlaywright.CreateInternalAsync(
                new NuGetPackageInstaller(findPackageByIdResource, packageSource),
                new PowerPlaywrightConfiguration
                {
                    PageObjectAssemblies = [new() { Path = "PowerPlaywright.TestApp.PageObjects.dll" }],
                });
        }

        /// <summary>
        /// Sets up tracing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task SetupTracingAsync()
        {
            await this.Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true,
            });

            this.isTracing = true;
        }

        /// <summary>
        /// Sets up the current culture.
        /// </summary>
        [SetUp]
        public void SetupCurrentCulture()
        {
            // Explicitly setting current culture to match test users. Ideally, we should add a feature to read test user's locale settings on login.
            CultureInfo.CurrentCulture = new CultureInfo("en-GB");
        }

        /// <summary>
        /// Tears down tracing for the test.
        /// </summary>
        /// <remarks>
        /// Stops the tracing and saves the trace to a file if the test failed.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [TearDown]
        public async Task TearDownTracingAsync()
        {
            if (!this.isTracing)
            {
                return;
            }

            var isFailed = TestContext.CurrentContext.Result.Outcome == ResultState.Error ||
                TestContext.CurrentContext.Result.Outcome == ResultState.Failure;

            var path = isFailed ? Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "playwright-traces",
                $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip") : null;

            await this.Context.Tracing.StopAsync(new()
            {
                Path = path,
            });

            if (path is null)
            {
                return;
            }

            TestContext.AddTestAttachment(path, "Playwright trace");
        }

        /// <summary>
        /// Logs a test user into the test app.
        /// </summary>
        /// <returns>The home page.</returns>
        protected Task<IModelDrivenAppPage> LoginAsync()
        {
            return this.powerPlaywright.LaunchAppAsync(
                this.Context, Configuration.Url, TestAppUniqueName, this.User.Username, this.User.Password);
        }

        /// <summary>
        /// Creates a record and then logs into the app and navigates to it.
        /// </summary>
        /// <param name="record">The record to create.</param>
        /// <returns>The form.</returns>
        protected async Task<IEntityRecordPage> LoginAndNavigateToRecordAsync(Entity record)
        {
            using (var client = this.GetServiceClient())
            {
                var deactivateOnCreate = record.GetAttributeValue<OptionSetValue>(nameof(pp_Record.statecode))?.Value == 1;

                if (deactivateOnCreate)
                {
                    record.Attributes.Remove(nameof(pp_Record.statecode));
                    record.Attributes.Remove(nameof(pp_Record.statuscode));
                }

                var recordId = await client.CreateAsync(record);

                if (deactivateOnCreate)
                {
                    await client.UpdateAsync(new Entity(record.LogicalName, recordId)
                    {
                        Attributes =
                        {
                            [nameof(pp_Record.statecode)] = new OptionSetValue(1),
                            [nameof(pp_Record.statuscode)] = new OptionSetValue(2),
                        },
                    });
                }
            }

            var page = await this.LoginAsync();

            return await page.ClientApi.NavigateToRecordAsync(record.LogicalName, record.Id);
        }

        /// <summary>
        /// Gets a service client instance authenticated as the application user.
        /// </summary>
        /// <returns>A service client instance.</returns>
        protected ServiceClient GetServiceClient()
        {
            return new ServiceClient(Configuration.Url, Configuration.ClientId, Configuration.ClientSecret, true);
        }

        /// <summary>
        /// Gets a user configuration. Iterates through all configured users for load-balancing purposes.
        /// </summary>
        /// <returns>The user configuration.</returns>
        private static UserConfiguration GetUser()
        {
            if (!UserEnumerator.MoveNext())
            {
                UserEnumerator.Reset();
                UserEnumerator.MoveNext();
            }

            return UserEnumerator.Current;
        }

        private static TestSuiteConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<ModelDrivenAppTests>()
                .AddEnvironmentVariables(EnvironmentVariablePrefix);

            var configurationRoot = config
                .Build();

            var keyVaultConfiguration = configurationRoot
                .GetSection("keyVault")
                .Get<KeyVaultConfiguration>();

            if (keyVaultConfiguration != null)
            {
                TokenCredential tokenCredential;

                if (!string.IsNullOrEmpty(keyVaultConfiguration.ClientSecret))
                {
                    tokenCredential = new ClientSecretCredential(
                        keyVaultConfiguration.TenantId,
                        keyVaultConfiguration.ClientId,
                        keyVaultConfiguration.ClientSecret);
                }
                else
                {
                    tokenCredential = new InteractiveBrowserCredential(
                        new InteractiveBrowserCredentialOptions
                        {
                            TenantId = keyVaultConfiguration.TenantId,
                            TokenCachePersistenceOptions = new TokenCachePersistenceOptions(),
                        });
                }

                config = new ConfigurationBuilder()
                    .AddAzureKeyVault(keyVaultConfiguration.Url, tokenCredential, new AzureKeyVaultConfigurationOptions { ReloadInterval = null })
                    .AddEnvironmentVariables(EnvironmentVariablePrefix)
                    .AddUserSecrets<ModelDrivenAppTests>();

                configurationRoot = config.Build();
            }

            var configuration = configurationRoot.Get<TestSuiteConfiguration>() ??
                throw new PowerPlaywrightException("The integration test suite has missing configuration values.");

            if (configuration.Users == null || !configuration.Users.Any())
            {
                throw new Exception("You have not configured any users for the tests.");
            }

            return configuration;
        }
    }
}