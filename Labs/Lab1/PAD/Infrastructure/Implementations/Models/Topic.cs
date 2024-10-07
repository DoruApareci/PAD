using Infrastructure.Interfaces.Models;

namespace Infrastructure.Implementations.Models
{
    public class Topic : ITopic
    {
        public Guid TopicID { get; set; }
        public string Name { get; set; }
    }
}
