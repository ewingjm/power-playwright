namespace PowerPlaywright
{
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
        private readonly ILogger<GlobalPackagesAssemblyProvider> logger;

        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalPackagesAssemblyProvider"/> class.
        /// </summary>
        /// <param name="sourceRepositoryProvider">The source repository provider.</param>
        /// <param name="packageIdentity">The package identity.</param>
        /// <param name="logger">The logger.</param>
        public GlobalPackagesAssemblyProvider(PackageIdentity packageIdentity, ILogger<GlobalPackagesAssemblyProvider> logger)
        {
            this.packageIdentity = packageIdentity;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Assembly GetAssembly()
        {
            if (this.assembly == null)
            {
                var settings = Settings.LoadDefaultSettings(null);
                var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(settings);

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