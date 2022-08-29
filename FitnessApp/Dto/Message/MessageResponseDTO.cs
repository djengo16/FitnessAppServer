namespace FitnessApp.Dto.Message
{
    public class MessageResponseDTO
    {
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Body { get; set; }
    }
}
