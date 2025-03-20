namespace PowerPlaywright
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.Logging;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;

    /// <summary>
    /// Retrieves a control assembly from the global packages folder.
    /// </summary>
    internal class GlobalPackagesAssemblyProvider : IAssemblyProvider
    {
        private readonly PackageIdentity packageIdentity;
        private readonly ISettings nugetSettings;
        private readonly ILogger<GlobalPackagesAssemblyProvider> logger;

        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalPackagesAssemblyProvider"/> class.
        /// </summary>
        /// <param name="packageIdentity">The package identity.</param>
        /// <param name="nugetSettings">The NuGet settings.</param>
        /// <param name="logger">The logger.</param>
        public GlobalPackagesAssemblyProvider(PackageIdentity packageIdentity, ISettings nugetSettings, ILogger<GlobalPackagesAssemblyProvider> logger)
        {
            this.packageIdentity = packageIdentity ?? throw new ArgumentNullException(nameof(packageIdentity));
            this.nugetSettings = nugetSettings ?? throw new ArgumentNullException(nameof(nugetSettings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public Assembly GetAssembly()
        {
            if (this.assembly == null)
            {
                var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(this.nugetSettings);

                var packagePath = Path.Combine(
                    globalPackagesFolder,
                    this.packageIdentity.Id.ToLowerInvariant(),
                    this.packageIdentity.Version.ToString());

                var assemblyPath = Path.Combine(packagePath, "lib", "netstandard2.0", "PowerPlaywright.Strategies.dll");

                this.assembly = Assembly.LoadFrom(assemblyPath);
            }

            return this.assembly;
        }
    }
}