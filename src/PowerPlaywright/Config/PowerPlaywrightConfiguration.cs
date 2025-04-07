namespace PowerPlaywright.Config
{
    using NuGet.Packaging.Core;
    using System.Collections.Generic;

    /// <summary>
    /// Configuration for Power Playwright.
    /// </summary>
    public class PowerPlaywrightConfiguration
    {
        /// <summary>
        /// The strategies package identity.
        /// </summary>
        internal PackageIdentity PackageIdentity { get; set; }

        /// <summary>
        /// Additional control assemblies.
        /// </summary>
        public IList<PageObjectAssemblyConfiguration> PageObjectAssemblies { get; set; }
    }
}
