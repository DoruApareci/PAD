using System.ComponentModel;
using System.Windows;
using Grpc.Net.Client;
using GrpcAgent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Reciever.Services;


namespace Reciever
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string Topic { get; set; } = "Topic";
        object _lock;
        public string Message { get; set; } = "Message";
        public string MBIP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5001;

        WebApplication app;

        public MainWindow()
        {
            //InitializeComponent();
            this.DataContext = this;
        }

        private void Init()
        {
            var builder = WebApplication.CreateBuilder();
            var url = $"http://{MBIP}:0";
            builder.WebHost.UseUrls(url);

            builder.Services.AddSingleton<NotififierService>(new NotififierService(this));

            builder.Services.AddGrpc();

            app = builder.Build();

            app.MapGrpcService<NotififierService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

            try
            {
                app.Start();
                Subscribe(app);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Subscribe(WebApplication app)
        {
            var channel = GrpcChannel.ForAddress($"http://{MBIP}:{Port}");
            var client = new Subscriber.SubscriberClient(channel);

            var server = app.Services.GetRequiredService<IServer>();
            var addressesFeature = server.Features.Get<IServerAddressesFeature>();
            var address = addressesFeature?.Addresses.FirstOrDefault();


            Message += $"\nSubscriber listening at {address}\n";

            var request = new SubscribeRequest { Topic = Topic, Address = address };

            try
            {
                var reply = client.Subscribe(request);
                Message += $"Subscribe reply: {reply.IsSuccess}\n";
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error subscribing: {e.Message}");
            }
        }

        public void AddMessage(string message)
        {
            lock (_lock)
            {
                this.Message += $"\nMessage received: {message}\n";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //app?.Dispose();  
            this.Close();
        }

    }
}