using Broker.Services;
using Broker.Services.Interfaces;
using Common;

namespace Broker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls(EndpointsConstants.BrokerAddress);

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.Services.AddSingleton<IMessageStorageService, MessageStorageService>();
            builder.Services.AddSingleton<IConnectionStorageService, ConnectionStorageService>();
            builder.Services.AddHostedService<SenderWorker>(); 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<PublisherService>();
            app.MapGrpcService<SubscriberService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}