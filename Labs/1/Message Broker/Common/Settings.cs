namespace Common
{
    public class Settings
    {
        //netstat -an |find /i "listening"

        //int workerThreads, completionPortThreads;
        //ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
        //Console.WriteLine($"Max worker threads: {workerThreads}, Max I/O threads: {completionPortThreads}");

        public static int BROCKER_PORT = 9000;
        public static string BROCKER_IP = "127.0.0.1";
        public static string SUBSCRIBE_STRING = "subscribe#";
    }
}
