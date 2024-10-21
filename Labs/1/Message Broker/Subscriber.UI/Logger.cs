
using System.Windows.Controls;

namespace Subscriber.UI
{
    public class Logger : ILogger
    {
        private TextBlock _logs;
        private readonly object _LogsLock = new();


        public Logger(TextBlock log)
        {
            _logs = log;
        }

        public void Log(string message)
        {
            _logs.Dispatcher.BeginInvoke((Action)(() => _logs.Text += message + "\n"));
        }
    }
}
