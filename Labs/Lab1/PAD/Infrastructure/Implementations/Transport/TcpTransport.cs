using Infrastructure.Implementations.Models;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Implementations.Transport
{
    public class TcpTransport : ITransport
    {
        public string Protocol => "TCP";
        private TcpListener _listener;
        private List<TcpClient> _clients = new List<TcpClient>();

        public event Action<IMessage> MessageReceived;

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 5000);
            _listener.Start();
            _listener.BeginAcceptTcpClient(OnClientAccepted, null);
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            try
            {
                var client = _listener.EndAcceptTcpClient(ar);
                _clients.Add(client);
                _listener.BeginAcceptTcpClient(OnClientAccepted, null);
                // Start listening for messages from this client
                BeginReadMessage(client);
            }
            catch (Exception)
            {

            }
        }

        private void BeginReadMessage(TcpClient client)
        {
            var buffer = new byte[4096];
            client.GetStream().BeginRead(buffer, 0, buffer.Length, OnMessageReceived, new { Client = client, Buffer = buffer });
        }

        private void OnMessageReceived(IAsyncResult ar)
        {
            var state = (dynamic)ar.AsyncState;
            var client = (TcpClient)state.Client;
            var buffer = (byte[])state.Buffer;

            var bytesRead = client.GetStream().EndRead(ar);
            if (bytesRead > 0)
            {
                var messageData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var message = JsonSerializer.Deserialize<Message>(messageData);
                MessageReceived?.Invoke(message);
                BeginReadMessage(client);
            }
        }

        public void SendMessage(IMessage message)
        {
            var serializedMessage = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            foreach (var client in _clients)
            {
                client.GetStream().Write(serializedMessage, 0, serializedMessage.Length);
            }
        }
    }
}
