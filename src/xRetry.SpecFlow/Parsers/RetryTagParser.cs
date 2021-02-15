using System.Text.RegularExpressions;

namespace xRetry.SpecFlow.Parsers
{
    public class RetryTagParser : IRetryTagParser
    {
        // unescaped: ^retry(\(([0-9]+)(,([0-9]+))?\))?$
        private readonly Regex regex = new Regex(
            $"^{Constants.RETRY_TAG}(\\(([0-9]+)(,([0-9]+))?\\))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly IRetrySettings retrySettings;

        public RetryTagParser(IRetrySettings retrySettings)
        {
            this.retrySettings = retrySettings;
        }

        public RetryTag Parse(string tag)
        {
            if (tag == null) return new RetryTag(retrySettings.MaxRetry, retrySettings.DelayBetweenRetriesMs);

            int? maxRetries = retrySettings.MaxRetry;
            int? delayBetweenRetriesMs = retrySettings.DelayBetweenRetriesMs;
            var match = regex.Match(tag);
            if (match.Success && match.Groups[2].Success)
            {
                maxRetries = int.Parse(match.Groups[2].Value);

                // Group 4 is delay between retries
                if (match.Groups[4].Success)
                {
                    delayBetweenRetriesMs = int.Parse(match.Groups[4].Value);
                }
            }

            return new RetryTag(maxRetries, delayBetweenRetriesMs);
        }
    }
}