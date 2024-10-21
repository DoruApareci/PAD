using Common;
using Grpc.Net.Client;
using GrpcAgent;
using System.ComponentModel;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string Topic { get; set; } = "Topic";
        public string Message { get; set; } = "Message";
        public string MBIP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5001;

        GrpcChannel channel;
        GrpcAgent.Publisher.PublisherClient client;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            SendButton.IsEnabled = false;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new PublishRequest() { Topic = Topic, Content = Message };
            try
            {
                var reply = await client.PublishMessageAsync(request);
                Message = "";
                Message = $"Publish reply: {reply.IsSuccess}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error publishing the message. {ex.Message}");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var adrress = $"http://{MBIP}:{Port}";
            channel = GrpcChannel.ForAddress(adrress);
            client = new GrpcAgent.Publisher.PublisherClient(channel);
            SendButton.IsEnabled = true;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.Clear();
            }
        }
    }
}