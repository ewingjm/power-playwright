namespace PowerPlaywright.UnitTests
{
    using System.Text.Json;
    using Microsoft.Extensions.Logging;
    using Microsoft.Playwright;
    using NSubstitute;
    using PowerPlaywright.Framework;

    /// <summary>
    /// Tests for the <see cref="EnvironmentInfoProvider"/> class.
    /// </summary>
    [TestFixture]
    public class EnvironmentInfoProviderTests
    {
        private ILogger<EnvironmentInfoProvider> logger;
        private EnvironmentInfoProvider environmentInfoProvider;
        private IPage page;
        private IAPIResponse getControlsResponse;
        private IDictionary<string, Version> controlVersions;

        /// <summary>
        /// Sets up the tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.logger = Substitute.For<ILogger<EnvironmentInfoProvider>>();
            this.environmentInfoProvider = new EnvironmentInfoProvider(this.logger);
            this.page = Substitute.For<IPage>();
            this.getControlsResponse = Substitute.For<IAPIResponse>();
            this.controlVersions = new Dictionary<string, Version>();

            this.MockValidDefaults();
        }

        /// <summary>
        /// Tears down the resolver.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [TearDown]
        public async Task TearDown()
        {
            await this.getControlsResponse.DisposeAsync();
        }

        /// <summary>
        /// Tests that <see cref="EnvironmentInfoProvider.PlatformVersion"/> returns null if not initialised.
        /// </summary>
        [Test]
        public void PlatformVersion_NotInitialised_ReturnsNull()
        {
            Assert.That(this.environmentInfoProvider.PlatformVersion, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="EnvironmentInfoProvider.ControlVersions"/> returns null if not initialised.
        /// </summary>
        [Test]
        public void ControlVersions_NotInitialised_ReturnsNull()
        {
            Assert.That(this.environmentInfoProvider.ControlVersions, Is.Null);
        }

        /// <summary>
        /// Tests that <see cref="EnvironmentInfoProvider.PlatformVersion"/> is non-null after initialisation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task PlatformVersion_Initialised_ReturnsNonNullValue()
        {
            await this.environmentInfoProvider.InitializeAsync(this.page);

            Assert.That(this.environmentInfoProvider.PlatformVersion, Is.Not.Null);
        }

        /// <summary>
        /// Tests that <see cref="EnvironmentInfoProvider.ControlVersions"/> is non-null after initialisation.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ControlVersions_Initialised_ReturnsNonNullValue()
        {
            await this.environmentInfoProvider.InitializeAsync(this.page);

            Assert.That(this.environmentInfoProvider.ControlVersions, Is.Not.Null);
        }

        /// <summary>
        /// Asserts that <see cref="EnvironmentInfoProvider.InitializeAsync(IPage)"/> throws an <see cref="ArgumentNullException"/> if the page argument is null.
        /// </summary>
        [Test]
        public void InitializeAsync_NullPage_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.environmentInfoProvider.InitializeAsync(null));
        }

        /// <summary>
        /// Asserts that <see cref="EnvironmentInfoProvider.InitializeAsync(IPage)"/> throws an <see cref="PowerPlaywrightException"/> if the status of the response when getting the controls is non-200.
        /// </summary>
        [Test]
        public void InitialiseAsync_Non200ResponseRetrievingControls_ThrowsPowerPlaywrightException()
        {
            this.getControlsResponse.Ok.Returns(false);

            Assert.ThrowsAsync<PowerPlaywrightException>(() => this.environmentInfoProvider.InitializeAsync(this.page));
        }

        private void MockValidDefaults()
        {
            this.page.EvaluateAsync<string>("Xrm.Utility.getGlobalContext().getVersion()")
                .Returns((i) => Task.FromResult("9.8.0.0"));

            var environmentUrl = $"https://{Guid.NewGuid()}.crm.dynamics.com";
            this.page.Url
                .Returns(environmentUrl);
            this.getControlsResponse.Ok
                .Returns(true);
            this.page.APIRequest.GetAsync($"{environmentUrl}/api/data/v9.2/customcontrols?$select=name,version")
                .Returns(this.getControlsResponse);
            this.getControlsResponse.JsonAsync()
                .Returns((i) => Task.Run<JsonElement?>(() =>
                {
                    var responseObject = new
                    {
                        value = this.controlVersions.Select(kvp => new
                        {
                            name = kvp.Key,
                            version = kvp.Value,
                        }),
                    };

                    return JsonDocument.Parse(JsonSerializer.Serialize(responseObject)).RootElement;
                }));
        }
    }
}
