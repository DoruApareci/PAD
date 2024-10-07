using Infrastructure.Implementations.Models;
using Infrastructure.Implementations.Serializers;
using Infrastructure.Implementations.Transport;
using Infrastructure.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Sender.UI.ViewModels
{
    public class SenderViewModel : INotifyPropertyChanged
    {
        private readonly ISender _sender;
        private readonly IMessageBroker _messageBroker;
        private ITransport _transport;
        private IMessageSerializer _serializer;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> Topics { get; } = new ObservableCollection<string>();
        public string NewTopic { get; set; }
        public string SelectedTopic { get; set; }
        public string MessageContent { get; set; }
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

        public ICommand AddTopicCommand { get; }
        public ICommand SendMessageCommand { get; }

        private bool CanAddTopic(object obj)
        {
            return NewTopic != null && !Topics.Contains(NewTopic);
        }

        private bool CanSendMessage(object obj)
        {
            return !string.IsNullOrWhiteSpace(SelectedTopic) && !string.IsNullOrWhiteSpace(MessageContent);
        }

        public SenderViewModel()
        {
            _transport = new TcpTransport(); // Default to TCP
            _serializer = new JsonSerializer(); // Default to JSON
            _messageBroker = new MessageBroker.BL.MessageBroker(_transport, new GrpcTransport());
            _sender = new BL.Sender(_messageBroker, _serializer, _transport);

            AddTopicCommand = new RelayCommand(new Action<object>(AddTopic), new Predicate<object>(CanAddTopic));
            SendMessageCommand = new RelayCommand(new Action<object>(SendMessage), new Predicate<object>(CanSendMessage));
        }

        private void AddTopic(object p)
        {
            if (!string.IsNullOrWhiteSpace(NewTopic) && !Topics.Contains(NewTopic))
            {
                Topics.Add(NewTopic);
                AddLog($"Added new topic: {NewTopic}");
                NewTopic = "";
            }
        }

        private void SendMessage(object p)
        {
            if (!string.IsNullOrWhiteSpace(SelectedTopic) && !string.IsNullOrWhiteSpace(MessageContent))
            {
                var message = new Message
                {
                    Topic = new Topic { Name = SelectedTopic },
                    Content = MessageContent,
                    Timestamp = DateTime.UtcNow
                };

                _sender.Send(message);
                AddLog($"Sent message in topic: {SelectedTopic}");
                MessageContent = "";
            }
        }

        private void AddLog(string logMessage)
        {
            Logs += $"[{DateTime.Now}] {logMessage}\n";
        }
    }
}
