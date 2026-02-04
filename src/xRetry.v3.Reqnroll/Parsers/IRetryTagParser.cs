namespace xRetry.v3.Reqnroll.Parsers;

public interface IRetryTagParser
{
    RetryTag Parse(string tag);
}