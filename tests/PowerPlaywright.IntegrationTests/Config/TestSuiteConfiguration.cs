namespace PowerPlaywright.IntegrationTests.Config
{
    /// <summary>
    /// Configuration for the integration tests.
    /// </summary>
    public class TestSuiteConfiguration
    {
        /// <summary>
        /// Gets or sets the URL of the Dataverse environment.
        /// </summary>
        public required Uri Url { get; set; }

        /// <summary>
        /// Gets or sets e or sets the test users.
        /// </summary>
        public required IEnumerable<UserConfiguration> Users { get; set; }
    }
}
