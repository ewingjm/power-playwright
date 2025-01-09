namespace PowerPlaywright.IntegrationTests.Config
{
    /// <summary>
    /// Configuration for Key Vault.
    /// </summary>
    public class KeyVaultConfiguration
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public required Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the tenant ID.
        /// </summary>
        public required string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the client ID.
        /// </summary>
        public required string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string? ClientSecret { get; set; }
    }
}