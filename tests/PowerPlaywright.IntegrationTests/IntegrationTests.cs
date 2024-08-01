namespace PowerPlaywright.IntegrationTests
{
    using Microsoft.Extensions.Configuration;
    using PowerPlaywright.IntegrationTests.Config;

    /// <summary>
    /// A base class for integration tests.
    /// </summary>
    public abstract class IntegrationTests : ContextTest
    {
        private ModelDrivenApp? modelDrivenApp;

        static IntegrationTests()
        {
            Configuration = new ConfigurationBuilder()
                .AddUserSecrets<ModelDrivenAppTests>()
                .AddEnvironmentVariables()
                .Build()
                .Get<TestSuiteConfiguration>() ?? throw new PowerPlaywrightException("The integration test suite has missing configuration values.");
        }

        /// <summary>
        /// Gets the test suite configuration.
        /// </summary>
        protected static TestSuiteConfiguration Configuration { get; private set; }

        /// <summary>
        /// Gets the model-driven app.
        /// </summary>
        protected ModelDrivenApp ModelDrivenApp
        {
            get
            {
                this.modelDrivenApp ??= ModelDrivenApp.Launch(this.Context);

                return this.modelDrivenApp;
            }
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

    }
}
