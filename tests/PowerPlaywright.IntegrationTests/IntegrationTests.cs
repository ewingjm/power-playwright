namespace PowerPlaywright.IntegrationTests
{
    using System;
    using System.Reflection;
    using Azure.Core;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Packaging;
    using NuGet.Packaging.Signing;
    using NuGet.Protocol.Core.Types;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.IntegrationTests.Config;

    /// <summary>
    /// A base class for integration tests.
    /// </summary>
    public abstract class IntegrationTests : ContextTest
    {
        /// <summary>
        /// The unique name of the app used for testing.
        /// </summary>
        protected const string TestAppUniqueName = "pp_UserInterfaceDemo";

        private static IEnumerator<UserConfiguration> userEnumerator;

        private UserConfiguration? user;
        private ModelDrivenApp modelDrivenApp;

        static IntegrationTests()
        {
            Configuration = GetConfiguration();

            userEnumerator = Configuration.Users.GetEnumerator();
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
        /// Gets the model-driven app instance.
        /// </summary>
        protected ModelDrivenApp ModelDrivenApp => this.modelDrivenApp;

        /// <summary>
        /// Sets up the local feed and model-driven app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [SetUp]
        public async Task SetUpAsync()
        {
            this.modelDrivenApp = await ModelDrivenApp.LaunchInternalAsync(
                this.Context,
                Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "packages"));
        }

        /// <summary>
        /// Tears down the model-driven app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous .</returns>
        [TearDown]
        public async Task TearDownAsync()
        {
            if (this.modelDrivenApp is null)
            {
                return;
            }

            await this.modelDrivenApp.DisposeAsync();
        }

        /// <summary>
        /// Logs a test user into the test app.
        /// </summary>
        /// <returns>The home page.</returns>
        protected Task<IModelDrivenAppPage> LoginAsync()
        {
            return this.ModelDrivenApp.LoginAsync(Configuration.Url, TestAppUniqueName, this.User.Username, this.User.Password);
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
                await client.CreateAsync(record);
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
            return new ServiceClient(Configuration.Url, Configuration.ClientId, Configuration.ClientSecret, false);
        }

        /// <summary>
        /// Gets a user configuration. Iterates through all configured users for load-balancing purposes.
        /// </summary>
        /// <returns>The user configuration.</returns>
        private static UserConfiguration GetUser()
        {
            if (!userEnumerator.MoveNext())
            {
                userEnumerator.Reset();
                userEnumerator.MoveNext();
            }

            return userEnumerator.Current;
        }

        private static TestSuiteConfiguration GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<ModelDrivenAppTests>()
                .AddEnvironmentVariables();

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

                config.AddAzureKeyVault(
                    keyVaultConfiguration.Url,
                    tokenCredential,
                    new AzureKeyVaultConfigurationOptions { ReloadInterval = null });

                configurationRoot = config.Build();
            }

            var configuration = configurationRoot.Get<TestSuiteConfiguration>() ??
                throw new PowerPlaywrightException("The integration test suite has missing configuration values.");

            if (!configuration.Users.Any())
            {
                throw new Exception("You have not configured any users for the tests.");
            }

            return configuration;
        }
    }
}