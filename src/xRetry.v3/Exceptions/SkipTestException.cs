using System;
using System.Runtime.Serialization;

namespace xRetry.v3.Exceptions
{
    [Serializable]
    public class SkipTestException : Exception
    {
        public readonly string? Reason;

        public SkipTestException(string? reason)
            : base("Test skipped. Reason: " + reason)
        {
            Reason = reason;
        }

        protected SkipTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Reason = info.GetString(nameof(Reason));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Reason), Reason);

            base.GetObjectData(info, context);
        }
    }
}
