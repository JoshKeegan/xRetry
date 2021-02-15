using System.IO;
using System.Text;
using Utf8Json;
using Utf8Json.Resolvers;

namespace xRetry.SpecFlow
{
    public interface IRetrySettings
    {
        bool isVerbose { get; set; }
        bool Global { get; set; }
        int MaxRetry { get; set; }
        int DelayBetweenRetriesMs { get; set; }
    }

    public class RetrySettings : IRetrySettings
    {
        public bool isVerbose { get; set; }
        public bool Global { get; set; }
        public int MaxRetry { get; set; } = 3;
        public int DelayBetweenRetriesMs { get; set; } = 5000;

        public static RetrySettings LoadConfiguration()
        {
            var retrySettings = new RetrySettings();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "retrySettings.json");
            if (!File.Exists(path))
            {
                var serializedBytes = JsonSerializer.Serialize(retrySettings);
                var serializedString = JsonSerializer.PrettyPrint(serializedBytes);
                File.WriteAllText(path, serializedString);
                return retrySettings;
            }
            
            retrySettings = JsonSerializer.Deserialize<RetrySettings>(File.ReadAllText(path));
            var logger = new Logger(retrySettings);
            logger.Log(retrySettings);

            return retrySettings;
        }
    }
}