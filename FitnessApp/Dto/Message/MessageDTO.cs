namespace FitnessApp.Dto.Message
{
    public class MessageDTO
    {
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Body { get; set; }
    }
}
