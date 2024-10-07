using Infrastructure.Implementations.Models;
using Infrastructure.Implementations.Serializers;
using Infrastructure.Implementations.Transport;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Reciever.UI.ViewModels
{
    public class ReceiverViewModel : INotifyPropertyChanged
    {
        private readonly IReceiver _receiver;
        private readonly IMessageBroker _messageBroker;
        private ITransport _transport;
        private IMessageSerializer _serializer;


        public ObservableCollection<string> AvailableTopics { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> SubscribedTopics { get; } = new ObservableCollection<string>();
        public ObservableCollection<IMessage> ReceivedMessages { get; } = new ObservableCollection<IMessage>();
        public string SelectedTopic { get; set; }
        public string Logs { get; set; } = "";

        public bool UseTcp
        {
            get => _transport is TcpTransport;
            set
            {
                if (value) _transport = new TcpTransport();
            }
        }

        public bool UseGrpc
        {
            get => _transport is GrpcTransport;
            set
            {
                if (value) _transport = new GrpcTransport();
            }
        }

        public bool UseJson
        {
            get => _serializer is JsonSerializer;
            set
            {
                if (value) _serializer = new JsonSerializer();
            }
        }

        public bool UseXml
        {
            get => _serializer is XmlSerializer;
            set
            {
                if (value) _serializer = new XmlSerializer();
            }
        }

        public ICommand SubscribeCommand { get; }
        public ICommand UnsubscribeCommand { get; }

        public ReceiverViewModel()
        {
            _transport = new TcpTransport(); // Default to TCP
            _serializer = new JsonSerializer(); // Default to JSON
            _messageBroker = new MessageBroker.BL.MessageBroker(_transport, new GrpcTransport());
            _receiver = new BL.Receiver(_messageBroker, _serializer, _transport);

            SubscribeCommand = new RelayCommand(Subscribe);
            UnsubscribeCommand = new RelayCommand(Unsubscribe);

            _receiver.MessageReceived += OnMessageReceived;

            LoadTopics();
            StartListeningForNewTopics();
        }

        private void StartListeningForNewTopics()
        {
            var timer = new System.Timers.Timer(5000); // Check every 5 seconds
            timer.Elapsed += (sender, e) => CheckForNewTopics();
            timer.Start();
        }

        private void CheckForNewTopics()
        {
            var currentTopics = _messageBroker.GetAvailableTopics();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var topic in currentTopics)
                {
                    if (!AvailableTopics.Contains(topic.Name))
                    {
                        AvailableTopics.Add(topic.Name);
                    }
                }
            });
        }

        private void LoadTopics()
        {
            var topics = _messageBroker.GetAvailableTopics();
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var topic in topics)
                {
                    AvailableTopics.Add(topic.Name);
                }
            });
        }


        private void Subscribe(object p)
        {
            if (!string.IsNullOrWhiteSpace(SelectedTopic) && !SubscribedTopics.Contains(SelectedTopic))
            {
                var topic = new Topic { Name = SelectedTopic };
                _receiver.Subscribe(topic);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SubscribedTopics.Add(SelectedTopic);
                    AddLog($"Subscribed to topic: {SelectedTopic}");
                });
            }
        }

        private void Unsubscribe(object p)
        {
            if (!string.IsNullOrWhiteSpace(SelectedTopic) && SubscribedTopics.Contains(SelectedTopic))
            {
                var topic = new Topic { Name = SelectedTopic };
                _receiver.Unsubscribe(topic);
                SubscribedTopics.Remove(SelectedTopic);
                AddLog($"Unsubscribed from topic: {SelectedTopic}");
            }
        }

        private void OnMessageReceived(IMessage message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ReceivedMessages.Add(message);
                AddLog($"Received message in topic: {message.Topic.Name}");
            });
        }

        private void AddLog(string logMessage)
        {
            Logs += $"[{DateTime.Now}] {logMessage}\n";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}