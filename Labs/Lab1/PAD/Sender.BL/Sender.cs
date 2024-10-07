using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;

namespace Sender.BL
{
    public class Sender : ISender
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageSerializer _serializer;
        private readonly ITransport _transport;

        public Sender(IMessageBroker messageBroker, IMessageSerializer serializer, ITransport transport)
        {
            _messageBroker = messageBroker;
            _serializer = serializer;
            _transport = transport;
        }

        public void Send(IMessage message)
        {
            var serializedMessage = _serializer.Serialize(message);
            _transport.SendMessage(message);
            _messageBroker.SendMessage(this, message);
        }
    }

}
