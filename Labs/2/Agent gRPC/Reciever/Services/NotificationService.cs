using Grpc.Core;
using GrpcAgent;

namespace Reciever.Services
{
    public class NotififierService : Notifier.NotifierBase, INotificationService
    {
        private readonly MainWindow _mainWindow;

        public NotififierService(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public override Task<NotifyReply> Notify(NotifyRequest request, ServerCallContext context)
        {
            _mainWindow.AddMessage(request.Content);

            return Task.FromResult(new NotifyReply { IsSuccess = true });
        }
    }
}
