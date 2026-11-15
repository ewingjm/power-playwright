namespace PowerPlaywright
{
    using System.Collections.Generic;
    using System.Text.Json;
    using PowerPlaywright.Framework;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// Provides access to string values that are platform versioned.
    /// </summary>
    internal class PlatformReference : IPlatformReference
    {
        private readonly IEnvironmentInfoProvider environmentInfoProvider;
        private readonly JsonSerializerOptions jsonSerializerOptions;

        private IDictionary<string, string> reference;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformReference"/> class.
        /// </summary>
        /// <param name="environmentInfoProvider">The environment info provider.</param>
        public PlatformReference(IEnvironmentInfoProvider environmentInfoProvider)
        {
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
                    using (var stream = typeof(RedirectionInfoProvider).Assembly.GetManifestResourceStream("PowerPlaywright.Strategies.platform-reference.json"))
                    {
                        if (stream == null)
                        {
                            throw new PowerPlaywrightException("The installed strategies assembly does not contain platform-reference.json.");
                        }

                        this.jsonSerializerOptions.Converters.Add(new VersionedDictionaryConverter(this.environmentInfoProvider.PlatformVersion));
                        this.reference = JsonSerializer.Deserialize<Dictionary<string, string>>(stream, this.jsonSerializerOptions);
                    }
                }

                return this.reference;
            }
        }
    }
}