namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Notification;
    using FitnessApp.Models;

    public interface INotificationsService
    {
        bool IsTrainingDayNotification(string userId, string planId);

        Task<Notification> CreateTrainingDayNotificationAsync(string userId);
        Task<Notification> CreateUnreadMessageNotification(string recipientId, string senderId);

        Task ViewNotificationAsync(int notificationId);

        bool CheckUnreadMessageNotificationExistence(string senderId, string recipientId);
        Task<List<NotificationResponseDTO>> GetAllByRecipientId(string recipientId);
    }
}
