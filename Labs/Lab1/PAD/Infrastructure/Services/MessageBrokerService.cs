using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure.Implementations.Adapters;
using Infrastructure.Interfaces.Models;
using System.Threading.Channels;


namespace Infrastructure.Services
{
    public class MessageBrokerService : MessageBroker.MessageBrokerBase
    {
        private readonly Channel<IMessage> _messageChannel;

        public MessageBrokerService()
        {
            _messageChannel = System.Threading.Channels.Channel.CreateUnbounded<IMessage>();
        }

        public event Action<IMessage> MessageReceived;

        public override Task<MessageResponse> SendMessage(MessageRequest request, ServerCallContext context)
        {
            var message = new Message
            {
                Topic = request.Topic,
                Content = request.Content,
                Timestamp = request.Timestamp
            };

            var messageAdapter = new MessageAdapter(message);

            MessageReceived?.Invoke(messageAdapter);
            _messageChannel.Writer.TryWrite(messageAdapter);

            return Task.FromResult(new MessageResponse
            {
                Success = true,
                Message = message
            });
        }

        public override async Task SubscribeToTopic(TopicRequest request, IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
        {
            var reader = _messageChannel.Reader;

            try
            {
                while (await reader.WaitToReadAsync(context.CancellationToken))
                {
                    while (reader.TryRead(out var message))
                    {
                        if (message.Topic.Name == request.TopicName)
                        {
                            var response = new MessageResponse
                            {
                                Success = true,
                                Message = new Message
                                {
                                    Topic = message.Topic.Name,
                                    Content = message.Content,
                                    Timestamp = Timestamp.FromDateTime(message.Timestamp.ToUniversalTime())
                                }
                            };

                            await responseStream.WriteAsync(response);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // The client has disconnected or the operation was cancelled
            }
        }
    }
}