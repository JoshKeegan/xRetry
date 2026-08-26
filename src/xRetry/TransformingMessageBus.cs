using Xunit.Abstractions;
using Xunit.Sdk;

namespace xRetry
{
    /// <summary>
    /// An xUnit message bus that transforms messages before passing them to the underlying message bus.
    /// </summary>
    public class TransformingMessageBus : IMessageBus
    {
        private readonly IMessageBus underlyingMessageBus;
        private readonly MessageTransformer messageTransformer;

        public TransformingMessageBus(IMessageBus underlyingMessageBus, MessageTransformer messageTransformer)
        {
            this.underlyingMessageBus = underlyingMessageBus;
            this.messageTransformer = messageTransformer;
        }

        public bool QueueMessage(IMessageSinkMessage rawMessage) =>
            underlyingMessageBus.QueueMessage(messageTransformer.Transform(rawMessage));

        public void Dispose()
        {
            // Do not dispose of the underlying message bus - it is an externally owned resource
        }
    }
}
