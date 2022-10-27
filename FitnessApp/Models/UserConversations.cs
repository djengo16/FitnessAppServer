namespace FitnessApp.Models
{
    public class UserConversations
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string ConversationId { get; set; }
        public Conversation Conversation { get; set; }
    }
}
