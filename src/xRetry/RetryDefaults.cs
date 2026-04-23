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

        private static readonly ConcurrentDictionary<string, RetryDefaults> cache =
            new ConcurrentDictionary<string, RetryDefaults>(StringComparer.Ordinal);

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

        public static RetryDefaults Load(string directory) =>
            cache.GetOrAdd(directory ?? string.Empty, load);

        private static RetryDefaults load(string directory)
        {
            string configFilePath = Path.Combine(directory, FILE_NAME);
            if (!File.Exists(configFilePath))
            {
                return new RetryDefaults();
            }

            RetryDefaults defaults = readConfigFile(configFilePath);
            defaults.validate(configFilePath);
            return defaults;
        }

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
    }
}
