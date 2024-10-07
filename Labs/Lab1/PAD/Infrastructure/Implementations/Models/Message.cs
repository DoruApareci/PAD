using Infrastructure.Interfaces.Models;

namespace Infrastructure.Implementations.Models
{
    public class Message : IMessage
    {
        public Guid MessageID { get; set; }

        public ITopic Topic { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
