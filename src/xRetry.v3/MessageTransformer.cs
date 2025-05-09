using System.Linq;
using xRetry.v3.Extensions;
using Xunit.Sdk;
using Xunit.v3;

namespace xRetry.v3
{
    public class MessageTransformer
    {
        private readonly string[] skipOnExceptionFullNames;

        public bool Skipped { get; private set; }

        public MessageTransformer(string[] skipOnExceptionFullNames)
        {
            this.skipOnExceptionFullNames = skipOnExceptionFullNames;
        }

        /// <summary>
        /// Transforms a message received from an xUnit test into another message, replacing it
        /// where necessary to add additional functionality, e.g. dynamic skipping
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public IMessageSinkMessage Transform(IMessageSinkMessage message)
        {
            // If this is a message saying that the test has been skipped, replace the message with skipping the test
            if (message is TestFailed failed && failed.ExceptionTypes.ContainsAny(skipOnExceptionFullNames))
            {
                Skipped = true;
                return new TestSkipped
                {
                    Reason = failed.Messages.FirstOrDefault() ?? "",
                    AssemblyUniqueID = null,
                    TestCollectionUniqueID = null,
                    TestClassUniqueID = null,
                    TestMethodUniqueID = null,
                    TestCaseUniqueID = null,
                    TestUniqueID = null,
                    ExecutionTime = 0,
                    FinishTime = default,
                    Output = null,
                    Warnings = new string[]
                        { }
                };
            }

            // Otherwise this isn't a message saying the test is skipped, follow usual intercept for replay later behaviour
            return message;
        }
    }
}
