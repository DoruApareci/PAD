using Infrastructure.Implementations.Transport;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MessageBroker.UI.ViewModels
{
    public class MessageBrokerViewModel : INotifyPropertyChanged
    {
        private readonly IMessageBroker _messageBroker;


        public ObservableCollection<string> ConnectedClients { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> ActiveTopics { get; } = new ObservableCollection<string>();
        public ObservableCollection<IMessage> MessageHistory { get; } = new ObservableCollection<IMessage>();

        private bool IsBrokerRunning = false;

        public bool CanStartBroker(object p)
        { return !IsBrokerRunning; }
        private void StartMB(object p)
        {
            IsBrokerRunning = true;
            _messageBroker.Start();
        }
        public bool CanStopBroker(object p)
        { return IsBrokerRunning; }
        private void StopMB(object p)
        {
            IsBrokerRunning = false;
            _messageBroker.Stop();
        }

        public ICommand StartBrokerCommand { get; }
        public ICommand StopBrokerCommand { get; }

        public MessageBrokerViewModel()
        {
            var tcpTransport = new TcpTransport();
            var grpcTransport = new GrpcTransport();
            _messageBroker = new BL.MessageBroker(tcpTransport, grpcTransport);

            StartBrokerCommand = new RelayCommand(new Action<object>(StartMB), CanStartBroker);
            StopBrokerCommand = new RelayCommand(new Action<object>(StopMB), CanStopBroker);

            // Subscribe to events to update UI
            // This is a simplified example and would need to be expanded
            tcpTransport.MessageReceived += OnMessageReceived;
            grpcTransport.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(IMessage message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageHistory.Add(message);
                if (!ActiveTopics.Contains(message.Topic.Name))
                {
                    ActiveTopics.Add(message.Topic.Name);
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
