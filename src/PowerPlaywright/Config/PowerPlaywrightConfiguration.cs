namespace PowerPlaywright.Config
{
    using System.Collections.Generic;
    using NuGet.Packaging.Core;

    /// <summary>
    /// Configuration for Power Playwright.
    /// </summary>
    public class PowerPlaywrightConfiguration
    {
        /// <summary>
        /// Gets or sets additional control assemblies.
        /// </summary>
        public IList<PageObjectAssemblyConfiguration> PageObjectAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the strategies package identity.
        /// </summary>
        internal PackageIdentity PackageIdentity { get; set; }
    }
}
