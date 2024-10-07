using Infrastructure.Interfaces.Models;

namespace Infrastructure.Interfaces
{
    public interface IMessageSerializer
    {
        string Format { get; }
        string Serialize(IMessage message);
        IMessage Deserialize(string data);
    }
}
