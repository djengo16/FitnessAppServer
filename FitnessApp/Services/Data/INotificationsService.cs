namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Notification;
    using FitnessApp.Models;

    public interface INotificationsService
    {
        Task<Notification> CreateTrainingDayNotificationAsync(string userId);
        Task<Notification> SetupTrainingDayNotification(string userId, string activePlanId);
        Task<Notification> CreateUnreadMessageNotification(string recipientId, string senderId);
        Task ViewNotificationAsync(int notificationId);
        bool CheckUnreadMessageNotificationExistence(string senderId, string recipientId);
        Task<List<NotificationResponseDTO>> GetAllByRecipientId(string recipientId);
    }
}
