using System;

namespace xRetry.v3.Reqnroll.Parsers;

public sealed class RetryTag(int? maxRetries, int? delayBetweenRetriesMs) : IEquatable<RetryTag>
{
    public readonly int? DelayBetweenRetriesMs = delayBetweenRetriesMs;
    public readonly int? MaxRetries = maxRetries;

    public bool Equals(RetryTag other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return MaxRetries == other.MaxRetries && DelayBetweenRetriesMs == other.DelayBetweenRetriesMs;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((RetryTag)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (MaxRetries.GetHashCode() * 397) ^ DelayBetweenRetriesMs.GetHashCode();
        }
    }
}