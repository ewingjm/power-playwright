namespace PowerPlaywright
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;

    /// <summary>
    /// An interface that provides NuGet package installation capabilities.
    /// </summary>
    internal interface INuGetPackageInstaller
    {
        /// <summary>
        /// Gets all package versions.
        /// </summary>
        /// <param name="packageId">The package ID.</param>
        /// <returns>A collection of package versions.</returns>
        Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId);

        /// <summary>
        /// Install a package into the global packages folder.
        /// </summary>
        /// <param name="packageIdentity">The package identity.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task InstallPackageAsync(PackageIdentity packageIdentity);
    }
}