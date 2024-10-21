using Common;
using Newtonsoft.Json;
using System.Text;

namespace Broker
{
    class PayloadHandler
    {
        public static void Handle(byte[] payloadBytes, ConnectionInfo connectionInfo)
        {
            var payloadString = Encoding.UTF8.GetString(payloadBytes);

            if (payloadString.StartsWith(Settings.SUBSCRIBE_STRING))
            {
                connectionInfo.Topic = payloadString.Split(Settings.SUBSCRIBE_STRING).LastOrDefault();
                ConnectionStorage.Add(connectionInfo);
            }
            else
            {
                Payload payload = JsonConvert.DeserializeObject<Payload>(payloadString);
                PayloadStorage.Add(payload);
            }

        }
    }
}
