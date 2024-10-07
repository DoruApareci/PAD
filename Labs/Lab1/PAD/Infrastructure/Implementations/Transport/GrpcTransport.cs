using Grpc.Core;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using Infrastructure.Services;

namespace Infrastructure.Implementations.Transport
{
    public class GrpcTransport : ITransport
    {
        public string Protocol => "gRPC";
        private Server _server;
        private readonly MessageBrokerService _service;
        private MessageBroker.MessageBrokerClient _client;

        public event Action<IMessage> MessageReceived;

        public GrpcTransport()
        {
            _service = new MessageBrokerService();
            _service.MessageReceived += (message) => MessageReceived?.Invoke(message);
        }

        public void Start()
        {
            _server = new Server
            {
                Services = { MessageBroker.BindService(_service) },
                Ports = { new ServerPort("localhost", 5001, ServerCredentials.Insecure) }
            };
            _server.Start();

            var channel = new Channel("localhost:5001", ChannelCredentials.Insecure);
            _client = new MessageBroker.MessageBrokerClient(channel);
        }

        public void Stop()
        {
            _server.ShutdownAsync().Wait();
        }

        public void SendMessage(IMessage message)
        {
            var request = new MessageRequest
            {
                Topic = message.Topic.Name,
                Content = message.Content,
                Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(message.Timestamp.ToUniversalTime())
            };

            try
            {
                if (_client == null)
                {
                    //no grpc cliets subscribed to this topic
                    return;
                }
                var response = _client?.SendMessage(request);
                // Handle the response if needed
            }
            catch (RpcException e)
            {
                Console.WriteLine($"Error sending message via gRPC: {e.Status}");
            }
        }
    }
}
