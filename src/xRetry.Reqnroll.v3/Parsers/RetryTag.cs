using System;

namespace xRetry.Reqnroll.v3.Parsers
{
    public class RetryTag : IEquatable<RetryTag>
    {
        public readonly int? MaxRetries;
        public readonly int? DelayBetweenRetriesMs;

        public RetryTag(int? maxRetries, int? delayBetweenRetriesMs)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }

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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RetryTag) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (MaxRetries.GetHashCode() * 397) ^ DelayBetweenRetriesMs.GetHashCode();
            }
        }
    }
}
