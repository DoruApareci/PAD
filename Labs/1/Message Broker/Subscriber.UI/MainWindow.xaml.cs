using Common;
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

namespace Subscriber.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public string Topic { get; set; }
        SubscriberSocket subscriberSocket;
        public string MBIP { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 9000;
        ILogger logger;
        Thread thread;


        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            logger = new Logger(Messages);
            Topic= "Topic";
        }

        private void Button_Click(object sender, RoutedEventArgs e) //subs
        {
            subscriberSocket = new(Topic.ToLower(), logger);
            thread = new(DoConnection);
            thread.Start();
        }

        private void DoConnection()
        {
            subscriberSocket.Connect(MBIP, Port);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}