using Common;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Receiver.Services;

namespace Receiver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.UseUrls(EndpointsConstants.SubscriberAddress);

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<NotificationService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Start();

            Subscribe(app);

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        private static void Subscribe(WebApplication app)
        {
            var channel = GrpcChannel.ForAddress(EndpointsConstants.BrokerAddress);
            var client = new Subscriber.SubscriberClient(channel);

            var server = app.Services.GetRequiredService<IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            var address = addressesFeature?.Addresses.FirstOrDefault();
            Console.WriteLine($"Subscriber listening at {address}");

            Console.Write("Enter the topic: ");
            var topic = Console.ReadLine();

            var request = new SubscribeRequest { Topic = topic, Address = address };

            try
            {
                var reply = client.Subscribe(request);
                Console.WriteLine($"Subscribe reply: {reply.IsSuccess}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error subscribing: {e.Message}");
            }
        }
    }
}