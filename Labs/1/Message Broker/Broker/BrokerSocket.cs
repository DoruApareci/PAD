using Common;
using System.Net;
using System.Net.Sockets;

namespace Broker
{
    class BrokerSocket
    {
        private Socket _socket;
        private const int CONECTION_LIMIT = 8;

        public BrokerSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(string ip, int port)
        {
            try
            {
                _socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
                _socket.Listen(CONECTION_LIMIT);
                Accept();
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Can't open a new message broker. {e.Message}");
            }
        }

        private void Accept()
        {
            _socket.BeginAccept(AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = new ConnectionInfo();
            Console.WriteLine("Accepted new connection...");

            try
            {
                connection.Socket = _socket.EndAccept(asyncResult);
                connection.Address = connection.Socket.RemoteEndPoint.ToString();
                Receive(connection);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't accept. {e.Message}");
            }
            finally
            {
                Accept();
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
                Console.WriteLine($"{e.Message}");
                Close(connection);
            }
        }

        private static void Close(ConnectionInfo connection)
        {
            var address = connection.Socket.RemoteEndPoint.ToString();

            ConnectionStorage.Remove(address);
            if (connection.Socket != null)
            {
                connection.Socket.Shutdown(SocketShutdown.Both);
                connection.Socket.Close();
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connection = asyncResult.AsyncState as ConnectionInfo;

            try
            {
                Socket senderSocket = connection.Socket;
                SocketError response;
                int buffSize = senderSocket.EndReceive(asyncResult, out response);

                if (response == SocketError.Success && buffSize > 0)
                {
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connection.Buffer, payload, payload.Length);

                    PayloadHandler.Handle(payload, connection);
                }
                else
                {
                    // If buffSize is 0, the client has closed the connection
                    Console.WriteLine("Client disconnected.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can't receive data. {e.Message}");
            }
            finally
            {
                Receive(connection);
            }
        }
    }
}
