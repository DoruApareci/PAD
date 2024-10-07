using Infrastructure.Interfaces.Models;

namespace Infrastructure.Interfaces
{
    public interface ISender
    {
        void Send(IMessage message);
    }
}
