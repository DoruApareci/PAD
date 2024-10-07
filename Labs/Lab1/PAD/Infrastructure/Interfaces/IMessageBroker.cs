using Infrastructure.Interfaces.Models;

namespace Infrastructure.Interfaces
{
    public interface IMessageBroker
    {
        public void Start();
        public void Stop();
        public void RegisterReceiver(IReceiver receiver, ITopic topic);
        public void UnregisterReceiver(IReceiver receiver, ITopic topic);
        public void SendMessage(ISender sender, IMessage message);
        IEnumerable<ITopic> GetAvailableTopics();
        public IEnumerable<IMessage> GetMessageHistory(ITopic topic);
    }
}
