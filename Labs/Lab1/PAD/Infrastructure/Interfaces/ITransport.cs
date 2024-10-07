using Infrastructure.Interfaces.Models;

namespace Infrastructure.Interfaces
{
    public interface ITransport
    {
        public string Protocol { get; }
        public void Start();
        public void Stop();
        public event Action<IMessage> MessageReceived;
        public void SendMessage(IMessage message);
    }
}
