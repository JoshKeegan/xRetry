using System.IO;
using System.Text;
using Utf8Json;

namespace xRetry.SpecFlow
{
    public interface ILogger
    {
        void Log(string msg);
        void Log<T>(T obj);
    }

    public class Logger : ILogger
    {
        private readonly IRetrySettings retrySettings;

        public Logger(IRetrySettings retrySettings)
        {
            this.retrySettings = retrySettings;
        }

        public void Log(string msg)
        {
            if (!retrySettings.isVerbose) return;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "retrySpecflowPlugin.log");
            using (var file = new StreamWriter(path, true))
            {
                file.WriteLine(msg);
            }
        }

        public void Log<T>(T obj)
        {
            var msg = JsonSerializer.PrettyPrint(JsonSerializer.Serialize(obj));
            Log(msg);
        }
    }
}