namespace xRetry.SpecFlow.Parsers
{
    public interface IRetryTagParser
    {
        RetryTag Parse(string tag);
    }
}
