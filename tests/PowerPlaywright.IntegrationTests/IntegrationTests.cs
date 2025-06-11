namespace PowerPlaywright.IntegrationTests
{
    using Azure.Core;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Extensions.Configuration;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Newtonsoft.Json;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NUnit.Framework.Interfaces;
    using PowerPlaywright.Api;
    using PowerPlaywright.Config;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Framework.Pages;
    using PowerPlaywright.IntegrationTests.Config;
    using PowerPlaywright.TestApp.Model.Search;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// A base class for integration tests.
    /// </summary>
    public abstract class IntegrationTests : ContextTest
    {
        /// <summary>
        /// The unique name of the app used for testing.
        /// </summary>
        protected const string TestAppUniqueName = "pp_UserInterfaceDemo";

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
        /// Creates a test record and retrieves it with all columns as a strongly-typed entity.
        /// </summary>
        /// <typeparam name="TEntity">The early-bound entity type.</typeparam>
        /// <param name="record">The record to create.</param>
        /// <returns>The created record with all columns as <typeparamref name="TEntity"/>.</returns>
        protected async Task<TEntity> CreateRecordAsync<TEntity>(TEntity record)
            where TEntity : Entity
        {
            using (var client = this.GetServiceClient())
            {
                var id = await client.CreateAsync(record);
                var retrieved = await client.RetrieveAsync(record.LogicalName, id, new ColumnSet(true));
                return retrieved.ToEntity<TEntity>();
            }
        }

        /// <summary>
        /// Creates a test record and retrieves it with all columns as a strongly-typed entity.
        /// </summary>
        /// <param name="searchText">The record to create.</param>
        /// <returns>SearchQueryApiResponse.</returns>
        /// <summary/>
        protected async Task<SearchQueryApiResponse> SearchAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ArgumentException("Search text cannot be null or empty.", nameof(searchText));
            }

            using var client = this.GetServiceClient();
            {
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri($"{Configuration.Url}api/data/v9.2/");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", client.CurrentAccessToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestBody = new
                {
                    search = searchText,
                    count = true,
                };

                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("searchquery", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SearchQueryApiResponse>(json) ?? new SearchQueryApiResponse();
                    return result;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Request failed: {response.StatusCode}, {error}");
                }
            }
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

            if (configuration.Users == null || !configuration.Users.Any())
            {
                throw new Exception("You have not configured any users for the tests.");
            }

            return configuration;
        }
    }
}