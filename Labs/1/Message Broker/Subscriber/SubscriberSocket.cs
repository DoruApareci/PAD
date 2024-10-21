using Common;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Subscriber
{
    public class SubscriberSocket
    {
        private Socket _socket;
        private string _topic;
        private readonly ILogger _logger;

        public SubscriberSocket(string topic, ILogger logger)
        {
            _topic = topic;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _logger = logger;
        }

        public void Connect(string ipAddress, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallback, null);
            _logger.Log("Waiting for connection...");
        }

        private void ConnectedCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = new ConnectionInfo();
            try
            {
                connection.Socket = _socket;
                if (_socket.Connected)
                {
                    _logger.Log("Subscriber connect to broker.");
                    Subscribe();
                    Receive(connection);
                }
                else
                {
                    _logger.Log("Subscriber could not connect to broker!");
                    Close(connection);
                }
            }
            catch (Exception e)
            {

                _logger.Log($"Subscriber could not connected to broker! {e.Message}");
            }
        }

        private void Receive(ConnectionInfo connection)
        {
            try
            {
                if (connection.Socket.Connected)
                {
                    connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length,
                        SocketFlags.None, ReceiveCallback, connection);
                }
                else
                {
                    Close(connection);
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{e.Message}");
                Close(connection);
            }
        }

        private static void Close(ConnectionInfo connection)
        {
            connection.Socket.Close();
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connectionInfo = asyncResult.AsyncState as ConnectionInfo;

            try
            {
                SocketError response;
                int buffsize = _socket.EndReceive(asyncResult, out response);

                if (response == SocketError.Success)
                {
                    byte[] payloadBytes = new byte[buffsize];

                    Array.Copy(connectionInfo.Buffer, payloadBytes, payloadBytes.Length);

                    var payloadString = Encoding.UTF8.GetString(payloadBytes);
                    var payload = JsonConvert.DeserializeObject<Payload>(payloadString);
                    _logger.Log($"Received message: {payload.Message}");
                    //PayloadHandler.Handle(payloadBytes);
                }
                else
                {
                    _logger.Log($"Can't receive data from broker.");
                }
            }
            catch (Exception e)
            {
                _logger.Log($"Can't receive data from broker. {e.Message}");
            }
            finally
            {
                Receive(connectionInfo);
            }
        }

        private void Subscribe()
        {
            var data = Encoding.UTF8.GetBytes(Settings.SUBSCRIBE_STRING + _topic);
            Send(data);
        }

        private void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch (Exception e)
            {
                _logger.Log($"Could not send data {e.Message}");
            }
        }
    }
}
