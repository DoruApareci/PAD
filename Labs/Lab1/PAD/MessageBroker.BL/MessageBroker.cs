using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using System.Collections.Concurrent;

namespace MessageBroker.BL
{
    public class MessageBroker : IMessageBroker
    {
        private readonly ConcurrentDictionary<ITopic, List<IReceiver>> _subscribers = new();
        private readonly ConcurrentDictionary<ITopic, List<IMessage>> _messageHistory = new();
        private readonly ITransport _tcpTransport;
        private readonly ITransport _grpcTransport;
        private readonly ConcurrentBag<ITopic> _availableTopics = new();


        public MessageBroker(ITransport tcpTransport, ITransport grpcTransport)
        {
            _tcpTransport = tcpTransport;
            _grpcTransport = grpcTransport;

            _tcpTransport.MessageReceived += OnMessageReceived;
            _grpcTransport.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            _tcpTransport.Start();
            _grpcTransport.Start();
        }

        public void Stop()
        {
            _tcpTransport.Stop();
            _grpcTransport.Stop();
        }

        public void RegisterReceiver(IReceiver receiver, ITopic topic)
        {
            _subscribers.AddOrUpdate(topic, new List<IReceiver> { receiver },
                (key, list) => { list.Add(receiver); return list; });
        }

        public void UnregisterReceiver(IReceiver receiver, ITopic topic)
        {
            if (_subscribers.TryGetValue(topic, out var receivers))
            {
                receivers.Remove(receiver);
            }
        }

        public IEnumerable<ITopic> GetAvailableTopics()
        {
            return _availableTopics.ToList();
        }

        public void RegisterTopic(ITopic topic)
        {
            if (!_availableTopics.Contains(topic))
            {
                _availableTopics.Add(topic);
            }
        }

        public void SendMessage(ISender sender, IMessage message)
        {
            // Register topic
            RegisterTopic(message.Topic);

            // Save message to history
            _messageHistory.AddOrUpdate(message.Topic, new List<IMessage> { message },
                (key, list) => { list.Add(message); return list; });

            // Dispatch message to receivers
            if (_subscribers.TryGetValue(message.Topic, out var receivers))
            {
                foreach (var receiver in receivers)
                {
                    receiver.Receive(message);
                }
            }

            // Send message through both transports
            _tcpTransport.SendMessage(message);
            _grpcTransport.SendMessage(message);
        }

        public IEnumerable<IMessage> GetMessageHistory(ITopic topic)
        {
            if (_messageHistory.TryGetValue(topic, out var messages))
            {
                return messages;
            }
            return Enumerable.Empty<IMessage>();
        }

        private void OnMessageReceived(IMessage message)
        {
            SendMessage(null, message);
        }
    }
}
