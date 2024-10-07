using Infrastructure.Interfaces.Models;
using Infrastructure.Services;

namespace Infrastructure.Implementations.Adapters
{
    public class MessageAdapter : IMessage
    {
        private readonly Message _grpcMessage;
        public Guid MessageID { get; set; }

        public MessageAdapter(Message grpcMessage)
        {
            _grpcMessage = grpcMessage;
        }

        public ITopic Topic => new TopicAdapter(_grpcMessage.Topic);
        public string Content => _grpcMessage.Content;
        public DateTime Timestamp => _grpcMessage.Timestamp.ToDateTime();

    }
}
