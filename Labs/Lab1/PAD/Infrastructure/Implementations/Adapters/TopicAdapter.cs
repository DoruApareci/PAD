using Infrastructure.Interfaces.Models;

namespace Infrastructure.Implementations.Adapters
{
    public class TopicAdapter : ITopic
    {
        public string Name { get; }

        public Guid TopicID { get; }

        public TopicAdapter(string topicName)
        {
            Name = topicName;
        }
    }
}
