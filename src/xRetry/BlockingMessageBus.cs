using System;
using System.Collections.Concurrent;
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
        private readonly MessageTransformer messageTransformer;
        private ConcurrentQueue<IMessageSinkMessage> messageQueue = new ConcurrentQueue<IMessageSinkMessage>();

        public BlockingMessageBus(IMessageBus underlyingMessageBus, MessageTransformer messageTransformer)
        {
            this.underlyingMessageBus = underlyingMessageBus;
            this.messageTransformer = messageTransformer;
        }

        public bool QueueMessage(IMessageSinkMessage rawMessage)
        {
            // Transform the message to apply any additional functionality, then intercept & store it for replay later
            IMessageSinkMessage transformedMessage = messageTransformer.Transform(rawMessage);
            messageQueue.Enqueue(transformedMessage);

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
        public void Flush(Func<IMessageSinkMessage, IMessageSinkMessage> messageRewriter)
        {
            while (messageQueue.TryDequeue(out IMessageSinkMessage message))
            {
                underlyingMessageBus.QueueMessage(messageRewriter(message));
            }
        }

        public void Dispose()
        {
            // Do not dispose of the underlying message bus - it is an externally owned resource
        }
    }
}
