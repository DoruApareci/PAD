using Common;

namespace Broker
{
    static class ConnectionStorage
    {
        private static List<ConnectionInfo> _connections;
        private static object _locker;

        static ConnectionStorage()
        {
            _connections = new List<ConnectionInfo>();
            _locker = new object();
        }

        public static void Add(ConnectionInfo info)
        {
            lock (_locker)
            {
                _connections.Add(info);
            }
        }

        public static void Remove(string address)
        {
            lock (_locker)
            {
                _connections.RemoveAll(x => x.Address == address);
            }
        }
        public static List<ConnectionInfo> GetConnectionsByTopic(string topic)
        {
            List<ConnectionInfo> selectedConnection;

            lock (_locker)
            {
                selectedConnection = _connections.Where(x => x.Topic == topic).ToList();
            }
            return selectedConnection;
        }
    }
}
