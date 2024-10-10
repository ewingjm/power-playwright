namespace PowerPlaywright.IntegrationTests.Config
{
    /// <summary>
    /// Configuration for a test user.
    /// </summary>
    public class UserConfiguration
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public required string Password { get; set; }
    }
}