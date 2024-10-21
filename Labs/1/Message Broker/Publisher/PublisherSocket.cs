using System.Net;
using System.Net.Sockets;

namespace Publisher
{
    public class PublisherSocket
    {
        private Socket _socket;
        public bool IsConnected { get; private set; }

        public PublisherSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(String ipAddres, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddres), port), ConnectedCallback, null);
            Thread.Sleep(2000);
        }

        private void ConnectedCallback(IAsyncResult asyncResult)
        {
            if (_socket.Connected)
            {
                Console.WriteLine("Sender connected to Broker");
            }
            else
            {
                Console.WriteLine("Error: Sender not connected to Broker");
            }
            IsConnected = _socket.Connected;
        }

        public void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send data. {e.Message}");
            }
        }
    }
}
