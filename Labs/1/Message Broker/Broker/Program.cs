using Common;

namespace Broker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Broker");

            BrokerSocket socket = new BrokerSocket();
            socket.Start(Settings.BROCKER_IP, Settings.BROCKER_PORT);

            var worker = new Worker();
            Task.Factory.StartNew(worker.DoSendMessageWork, TaskCreationOptions.LongRunning);

            Console.ReadLine();
        }
    }
}
