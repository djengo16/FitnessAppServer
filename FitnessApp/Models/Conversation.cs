namespace FitnessApp.Models
{
    public class Conversation
    {
        public Conversation()
        {

            this.Id = Guid.NewGuid().ToString();
            this.Messages = new HashSet<Message>();
            this.UserConversations = new HashSet<UserConversations>();
        }
        public string Id { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<UserConversations> UserConversations { get; set; }
    }
}
