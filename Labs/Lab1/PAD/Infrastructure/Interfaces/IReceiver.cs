using Infrastructure.Interfaces.Models;

namespace Infrastructure.Interfaces
{
    public interface IReceiver
    {
        public void Receive(IMessage message);
        public void Subscribe(ITopic topic);
        public void Unsubscribe(ITopic topic);
        public event Action<IMessage> MessageReceived;
    }
}
