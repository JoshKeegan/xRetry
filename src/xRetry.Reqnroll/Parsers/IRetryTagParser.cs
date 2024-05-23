namespace xRetry.Reqnroll.Parsers
{
    public interface IRetryTagParser
    {
        RetryTag Parse(string tag);
    }
}
