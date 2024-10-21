using Common;
using Newtonsoft.Json;
using System.ComponentModel;
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

namespace Publisher.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        PublisherSocket publisherSocket;
        Payload payload;
        public string Topic { get; set; } = "Topic";
        public string Message { get; set; } = "Message";
        public string MBIP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 9000;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (publisherSocket.IsConnected)
            {
                var payload = new Payload();

                payload.Topic = this.Topic.ToLower();

                payload.Message = Message;

                var payloadString = JsonConvert.SerializeObject(payload);
                byte[] data = Encoding.UTF8.GetBytes(payloadString);

                publisherSocket.Send(data);
            }
            else
            {
                MessageBox.Show("Not connected to the brocker");
            }
            Message = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                publisherSocket = new();
                publisherSocket.Connect(MBIP, Port);
                payload = new Payload();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}