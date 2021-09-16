using System.Collections.Concurrent;
using System.Linq;
using xRetry.Exceptions;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    /// <summary>
    /// An XUnit message bus that can block messages from being passed until we want them to be.
    /// </summary>
    public class BlockingMessageBus : IMessageBus
    {
        private readonly IMessageBus underlyingMessageBus;
        private ConcurrentQueue<IMessageSinkMessage> messageQueue = new ConcurrentQueue<IMessageSinkMessage>();
        public bool Skipped { get; private set; } = false;

        public BlockingMessageBus(IMessageBus underlyingMessageBus)
        {
            this.underlyingMessageBus = underlyingMessageBus;
        }

        public bool QueueMessage(IMessageSinkMessage message)
        {
            // If this is a message saying that the test has been skipped, we can interrupt execution at this point
            if (message is TestFailed failed && failed.ExceptionTypes.Contains(typeof(SkipTestException).FullName))
            {
                string reason = failed.Messages?.FirstOrDefault();
                messageQueue.Enqueue(new TestSkipped(failed.Test, reason));
                Skipped = true;
            }
            else
            {
                // Otherwise this isn't a message saying the test is skipped, follow usual intercept & replay later behaviour
                messageQueue.Enqueue(message);
            }

            // Returns if execution should continue. Since we are intercepting the message, we
            //  have no way of checking this so always continue...
            return true;
        }

        public void Clear()
        {
            messageQueue = new ConcurrentQueue<IMessageSinkMessage>();
        }

        /// <summary>
        /// Write the cached messages to the underlying message bus
        /// </summary>
        public void Flush()
        {
            while (messageQueue.TryDequeue(out IMessageSinkMessage message))
            {
                underlyingMessageBus.QueueMessage(message);
            }
        }

        public void Dispose()
        {
            // Do not dispose of the underlying message bus - it is an externally owned resource
        }
    }
}
