using Grpc.Core;
using GrpcAgent;

namespace Reciever.Services;

public interface INotificationService
{
    Task<NotifyReply> Notify(NotifyRequest request, ServerCallContext context);
}
