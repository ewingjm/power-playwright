namespace PowerPlaywright
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A converter for a versioned dictionary.
    /// </summary>
    public class VersionedDictionaryConverter : JsonConverter<Dictionary<string, string>>
    {
        private readonly Version targetVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedDictionaryConverter"/> class.
        /// </summary>
        /// <param name="targetVersion">The target version.</param>
        public VersionedDictionaryConverter(Version targetVersion)
        {
            this.targetVersion = targetVersion;
        }

        /// <inheritdoc/>
        public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new Dictionary<string, string>();

            var nestedDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<Version, string>>>(ref reader, options);

            if (nestedDict != null)
            {
                foreach (var topLevelKvp in nestedDict)
                {
                    var topLevelKey = topLevelKvp.Key;
                    var versionedValues = topLevelKvp.Value
                        .Where(kvp => kvp.Key <= this.targetVersion)
                        .OrderByDescending(v => v.Key)
                        .FirstOrDefault();

                    if (!versionedValues.Equals(default(KeyValuePair<string, string>)))
                    {
                        result[topLevelKey] = versionedValues.Value;
                    }
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Serialization is not supported.");
        }
    }
}
