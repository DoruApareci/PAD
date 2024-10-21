using Common;
using Newtonsoft.Json;
using System.Text;

namespace Subscriber
{
    class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);
            var payload = JsonConvert.DeserializeObject<Payload>(payloadString);

            Console.WriteLine(payload.Message);
        }
    }
}
