using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;

namespace Reciever.BL
{
    public class Receiver : IReceiver
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IMessageSerializer _serializer;
        private readonly ITransport _transport;

        public event Action<IMessage> MessageReceived;

        public Receiver(IMessageBroker messageBroker, IMessageSerializer serializer, ITransport transport)
        {
            _messageBroker = messageBroker;
            _serializer = serializer;
            _transport = transport;
            _transport.MessageReceived += OnTransportMessageReceived;
        }

        public void Subscribe(ITopic topic)
        {
            _messageBroker.RegisterReceiver(this, topic);
            // Retrieve missed messages
            var history = _messageBroker.GetMessageHistory(topic);
            foreach (var message in history)
            {
                Receive(message);
            }
        }

        public void Unsubscribe(ITopic topic)
        {
            _messageBroker.UnregisterReceiver(this, topic);
        }

        public void Receive(IMessage message)
        {
            var deserializedMessage = _serializer.Deserialize(_serializer.Serialize(message));
            MessageReceived?.Invoke(deserializedMessage);
        }

        private void OnTransportMessageReceived(IMessage message)
        {
            Receive(message);
        }
    }
}
 