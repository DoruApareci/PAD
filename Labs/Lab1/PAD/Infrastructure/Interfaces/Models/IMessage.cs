namespace Infrastructure.Interfaces.Models
{
    public interface IMessage
    {
        public Guid MessageID { get; set; }
        public ITopic Topic { get; }
        public string Content { get; }
        public DateTime Timestamp { get; }
    }
}
