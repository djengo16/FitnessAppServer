using FitnessApp.Models.Enums;

namespace FitnessApp.Dto.Notification
{
    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsViewed { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public string RecepientId { get; set; }
    }
}
