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
    using PowerPlaywright.IntegrationTests.Extensions;
    using PowerPlaywright.TestApp.Model.Search;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

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
        /// Retrieves all records matching the specified query, handling paging automatically.
        /// </summary>
        /// <param name="query">The query used to retrieve the records. Must be a <see cref="QueryExpression"/>.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains an <see cref="EntityCollection"/> with all matching records.
        /// </returns>
        protected async Task<EntityCollection> RetrieveRecordsAsync(QueryBase query)
        {
            if (query is not QueryExpression queryExpression)
            {
                throw new ArgumentException("Only QueryExpression is supported for paging.", nameof(query));
            }

            using var client = this.GetServiceClient();

            queryExpression.PageInfo ??= new PagingInfo
            {
                PageNumber = 1,
                Count = 5000,
                PagingCookie = null,
            };

            var allRecords = new EntityCollection
            {
                EntityName = queryExpression.EntityName,
            };

            do
            {
                var result = await client.RetrieveMultipleAsync(queryExpression);

                allRecords.Entities.AddRange(result.Entities);

                if (!result.MoreRecords)
                {
                    break;
                }

                queryExpression.PageInfo.PageNumber++;
                queryExpression.PageInfo.PagingCookie = result.PagingCookie;
            } while (true);

            return allRecords;
        }

        /// <summary>
        /// Creates a test record and retrieves it with all columns as a strongly-typed entity.
        /// </summary>
        /// <param name="searchText">The record to create.</param>
        /// <param name="entitiesToSearch">The entities to search by.</param>
        /// <param name="topCount">The top number of records to return.</param>
        /// <returns>SearchQueryApiResponse.</returns>
        protected async Task<SearchQueryApiResponse?> SearchAsync(string searchText, string[] entitiesToSearch, int topCount = 50)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ArgumentException("Search text cannot be null or empty.", nameof(searchText));
            }

            using var client = this.GetServiceClient();
            using var httpClient = CreateHttpClient(client.CurrentAccessToken);

            var requestBody = new
            {
                search = searchText,
                count = true,
                top = topCount,
                entities = entitiesToSearch.ToSearchArray(),
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("searchquery", content);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SearchQueryApiResponse>(json) ?? new SearchQueryApiResponse();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(10000);
                return null;
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed: {response.StatusCode}, {error}");
        }

        /// <summary>
        /// Creates a http client for using the WebAPI.
        /// </summary>
        /// <param name="accessToken">The bearer token to send in the auth header.</param>
        /// <returns>A HttpClient instance.</returns>
        private static HttpClient CreateHttpClient(string accessToken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri($"{Configuration.Url}api/data/v9.2/"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
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