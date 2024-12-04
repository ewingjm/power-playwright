namespace PowerPlaywright
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.Json;
    using PowerPlaywright.Framework;

    /// <summary>
    /// Provides access to string values that are platform versioned.
    /// </summary>
    internal class PlatformReference : IPlatformReference
    {
        private static readonly string PlatformReferenceLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "platform-reference.json");

        private readonly IEnvironmentInfoProvider environmentInfoProvider;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        private IDictionary<string, string> reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformReference"/> class.
        /// </summary>
        /// <param name="environmentInfoProvider">The environment info provider.</param>
        public PlatformReference(IEnvironmentInfoProvider environmentInfoProvider)
        {
            this.environmentInfoProvider = environmentInfoProvider;
            this.jsonSerializerOptions = new JsonSerializerOptions();
            this.jsonSerializerOptions.Converters.Add(new VersionedDictionaryConverter(this.environmentInfoProvider.PlatformVersion));
        }

        /// <inheritdoc/>
        public string EntityListPageGridControlName => this.Reference["EntityListPage-GridControl-Name"];

        private IDictionary<string, string> Reference
        {
            get
            {
                if (this.reference == null)
                {
                    if (!File.Exists(PlatformReferenceLocation))
                    {
                        throw new PowerPlaywrightException("A platform-reference.json file was not found.");
                    }

                    this.reference = JsonSerializer.Deserialize<Dictionary<string, string>>(
                        PlatformReferenceLocation, this.jsonSerializerOptions);
                }

                return this.reference;
            }
        }
    }
}