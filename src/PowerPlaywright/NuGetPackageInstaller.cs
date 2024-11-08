namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.Packaging.Signing;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using PowerPlaywright.Framework;

    /// <summary>
    /// Provides NuGet installation functionality.
    /// </summary>
    internal class NuGetPackageInstaller : INuGetPackageInstaller
    {
        private const string StrategiesPackageId = "PowerPlaywright.Strategies";

        private readonly FindPackageByIdResource findPackageByIdResource;
        private readonly string source;
        private SourceCacheContext cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackageInstaller"/> class.
        /// </summary>
        /// <param name="findPackageByIdResource">The resource.</param>
        /// <param name="source">The source.</param>
        public NuGetPackageInstaller(FindPackageByIdResource findPackageByIdResource, string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException($"'{nameof(source)}' cannot be null or empty.", nameof(source));
            }

            this.findPackageByIdResource = findPackageByIdResource ?? throw new ArgumentNullException(nameof(findPackageByIdResource));
            this.source = source;
        }

        private SourceCacheContext Cache => this.cache ?? (this.cache = new SourceCacheContext());

        /// <inheritdoc/>
        public async Task<IEnumerable<NuGetVersion>> GetAllVersionsAsync(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                throw new ArgumentException($"'{nameof(packageId)}' cannot be null or empty.", nameof(packageId));
            }

            return await this.findPackageByIdResource.GetAllVersionsAsync(
                StrategiesPackageId,
                this.Cache,
                NullLogger.Instance,
                CancellationToken.None);
        }

        /// <inheritdoc/>
        public async Task InstallPackageAsync(PackageIdentity packageIdentity)
        {
            if (packageIdentity is null)
            {
                throw new ArgumentNullException(nameof(packageIdentity));
            }

            using (var packageStream = new MemoryStream())
            {
                var copiedToStream = await this.findPackageByIdResource.CopyNupkgToStreamAsync(
                    StrategiesPackageId,
                    packageIdentity.Version,
                    packageStream,
                    this.Cache,
                    NullLogger.Instance,
                    CancellationToken.None);

                if (!copiedToStream)
                {
                    throw new PowerPlaywrightException($"Failed to download version {packageIdentity.Version} of the {packageIdentity.Id} package.");
                }

                packageStream.Seek(0, SeekOrigin.Begin);

                var settings = Settings.LoadDefaultSettings(null);

                await GlobalPackagesFolderUtility.AddPackageAsync(
                    this.source,
                    packageIdentity,
                    packageStream,
                    SettingsUtility.GetGlobalPackagesFolder(settings),
                    parentId: Guid.Empty,
                    ClientPolicyContext.GetClientPolicy(settings, NullLogger.Instance),
                    NullLogger.Instance,
                    CancellationToken.None);
            }
        }
    }
}