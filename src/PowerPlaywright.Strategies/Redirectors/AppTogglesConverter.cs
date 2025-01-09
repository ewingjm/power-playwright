namespace PowerPlaywright
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Text.Json.Serialization;
    using PowerPlaywright.Strategies.Redirectors;

    /// <summary>
    /// A converter for a versioned dictionary.
    /// </summary>
    internal class AppTogglesConverter : JsonConverter<AppToggles>
    {
        private readonly Guid appId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppTogglesConverter"/> class.
        /// </summary>
        /// <param name="appId">The app ID.</param>
        public AppTogglesConverter(Guid appId)
        {
            this.appId = appId;
        }

        /// <inheritdoc/>
        public override AppToggles Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var tryToggleSets = reader.GetString();

            if (!string.IsNullOrEmpty(tryToggleSets) && JsonNode.Parse(tryToggleSets).AsObject().TryGetPropertyValue(this.appId.ToString(), out var appTogglesJson))
            {
                return appTogglesJson.Deserialize<AppToggles>();
            }

            return new AppToggles();
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, AppToggles value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization is not supported.");
        }
    }
}
