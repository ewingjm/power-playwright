namespace PowerPlaywright.Config
{
    using System.Collections.Generic;

    /// <summary>
    /// Configuration for Power Playwright.
    /// </summary>
    public class PowerPlaywrightConfiguration
    {
        /// <summary>
        /// Gets or sets the additional control assemblies.
        /// </summary>
        public IList<PageObjectAssemblyConfiguration> PageObjectAssemblies { get; set; }
    }
}
