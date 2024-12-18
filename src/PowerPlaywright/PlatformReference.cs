namespace PowerPlaywright
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using PowerPlaywright.Framework;

    /// <summary>
    /// Provides access to string values that are platform versioned.
    /// </summary>
    internal class PlatformReference : IPlatformReference
    {
        private readonly PackageIdentity packageIdentity;
        private readonly ISettings nugetSettings;
        private readonly IEnvironmentInfoProvider environmentInfoProvider;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        private IDictionary<string, string> reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformReference"/> class.
        /// </summary>
        /// <param name="packageIdentity">The strategies package identity.</param>
        /// <param name="nugetSettings">The NuGet settings.</param>
        /// <param name="environmentInfoProvider">The environment info provider.</param>
        public PlatformReference(PackageIdentity packageIdentity, ISettings nugetSettings, IEnvironmentInfoProvider environmentInfoProvider)
        {
            this.packageIdentity = packageIdentity ?? throw new System.ArgumentNullException(nameof(packageIdentity));
            this.nugetSettings = nugetSettings ?? throw new System.ArgumentNullException(nameof(nugetSettings));
            this.environmentInfoProvider = environmentInfoProvider ?? throw new System.ArgumentNullException(nameof(environmentInfoProvider));

            this.jsonSerializerOptions = new JsonSerializerOptions();
        }

        /// <inheritdoc/>
        public string EntityListPageGridControlName => this.Reference["EntityListPage-GridControl-Name"];

        private IDictionary<string, string> Reference
        {
            get
            {
                if (this.reference == null)
                {
                    var globalPackagesFolder = SettingsUtility.GetGlobalPackagesFolder(this.nugetSettings);

                    var packagePath = Path.Combine(
                        globalPackagesFolder,
                        this.packageIdentity.Id.ToLowerInvariant(),
                        this.packageIdentity.Version.ToString());

                    var referencePath = Path.Combine(packagePath, "content", "platform-reference.json");

                    if (!File.Exists(referencePath))
                    {
                        throw new PowerPlaywrightException("A platform-reference.json file was not found.");
                    }

                    this.jsonSerializerOptions.Converters.Add(new VersionedDictionaryConverter(this.environmentInfoProvider.PlatformVersion));
                    this.reference = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        File.ReadAllText(referencePath), this.jsonSerializerOptions);
                }

                return this.reference;
            }
        }
    }
}