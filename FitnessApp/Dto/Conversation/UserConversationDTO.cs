namespace FitnessApp.Dto.Conversation
{
    using FitnessApp.Dto.Message;
    public class ConversationDetailsDTO
    {
        public string Id { get; set; }

        public ICollection<MessageDTO> Messages { get; set; }

    }
}
