using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using Infrastructure.Implementations.Models;

namespace Infrastructure.Implementations.Serializers
{
    public class XmlSerializer : IMessageSerializer
    {
        public string Format => "XML";

        public string Serialize(IMessage message)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Message));
            using var writer = new StringWriter();
            serializer.Serialize(writer, message);
            return writer.ToString();
        }

        public IMessage Deserialize(string data)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Message));
            using var reader = new StringReader(data);
            return (IMessage)serializer.Deserialize(reader);
        }
    }
}
