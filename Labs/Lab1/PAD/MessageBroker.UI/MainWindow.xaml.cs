using MessageBroker.UI.ViewModels;
using System.Windows;

namespace MessageBroker.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MessageBrokerViewModel();
        }
    }
}