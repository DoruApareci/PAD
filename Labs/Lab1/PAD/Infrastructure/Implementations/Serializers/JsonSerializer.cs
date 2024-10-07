using Infrastructure.Implementations.Models;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;

namespace Infrastructure.Implementations.Serializers
{
    public class JsonSerializer : IMessageSerializer
    {
        public string Format => "JSON";

        public string Serialize(IMessage message)
        {
            return System.Text.Json.JsonSerializer.Serialize(message);
        }

        public IMessage Deserialize(string data)
        {
            return System.Text.Json.JsonSerializer.Deserialize<Message>(data);
        }
    }
}
