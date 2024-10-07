namespace Infrastructure.Interfaces.Models
{
    public interface ITopic
    {
        public Guid TopicID { get; }
        public string Name { get; }
    }
}
