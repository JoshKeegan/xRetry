using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace xRetry
{
    /// <summary>
    /// Project-level retry defaults loaded from <c>xretry.json</c>.
    /// </summary>
    public class RetryDefaults
    {
        public const string FILE_NAME = "xretry.json";

        // Case sensitivity is not determined by the OS: Windows can configure case-sensitive
        // directories, while NTFS or FAT32 volumes mounted on Linux can be case-insensitive.
        // Ordinal avoids conflating distinct paths; equivalent paths on a case-insensitive file
        // system may get separate cache entries.
        private static readonly ConcurrentDictionary<string, LoadResult> cache =
            new ConcurrentDictionary<string, LoadResult>(StringComparer.Ordinal);

        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            MissingMemberHandling = MissingMemberHandling.Error
        };

        [JsonProperty("maxRetries")]
        public int? MaxRetries { get; private set; }

        [JsonProperty("delayBetweenRetriesMs")]
        public int? DelayBetweenRetriesMs { get; private set; }

        [JsonProperty("retryUntaggedScenarios")]
        public bool RetryUntaggedScenarios { get; private set; }

        /// <summary>
        /// The JSON schema used by editors to validate <c>xretry.json</c>.
        /// This value has no effect at runtime.
        /// </summary>
        [JsonProperty("$schema")]
        public string Schema { get; private set; }

        internal static LoadResult Load(string directory) =>
            cache.GetOrAdd(
                directory ?? string.Empty,
                static configDirectory =>
                {
                    try
                    {
                        string configFilePath = Path.Combine(configDirectory, FILE_NAME);
                        if (!File.Exists(configFilePath))
                        {
                            return new LoadResult(new RetryDefaults());
                        }

                        RetryDefaults defaults = readConfigFile(configFilePath);
                        defaults.validate(configFilePath);
                        return new LoadResult(defaults);
                    }
                    catch (InvalidOperationException ex)
                    {
                        return new LoadResult(ex.Message);
                    }
                });

        private static RetryDefaults readConfigFile(string configFilePath)
        {
            try
            {
                string configFileContents = File.ReadAllText(configFilePath);
                if (string.IsNullOrWhiteSpace(configFileContents))
                {
                    throw new JsonException("Configuration file is empty.");
                }

                return JsonConvert.DeserializeObject<RetryDefaults>(
                    configFileContents,
                    serializerSettings) ?? throw new JsonException("Configuration must be a JSON object.");
            }
            catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
            {
                throw invalidConfigurationException(
                    configFilePath,
                    "could not be read as valid JSON configuration: " + ex.Message,
                    ex);
            }
        }

        private void validate(string configFilePath)
        {
            if (MaxRetries < 1)
            {
                throw invalidConfigurationException(configFilePath, "maxRetries must be >= 1");
            }

            if (DelayBetweenRetriesMs < 0)
            {
                throw invalidConfigurationException(configFilePath, "delayBetweenRetriesMs must be >= 0");
            }
        }

        private static InvalidOperationException invalidConfigurationException(
            string configFilePath,
            string message,
            Exception innerException = null) =>
            new($"xRetry configuration file \"{configFilePath}\" is invalid: {message}.", innerException);

        internal sealed class LoadResult
        {
            public LoadResult(RetryDefaults value)
            {
                Value = value;
            }

            public LoadResult(string error)
            {
                Value = new RetryDefaults();
                Error = error;
            }

            public RetryDefaults Value { get; }

            public string Error { get; }
        }
    }
}
