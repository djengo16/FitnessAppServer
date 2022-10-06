namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Notification;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;

    public interface INotificationsService
    {
        Task<Notification> SetupTrainingDayNotification(string userId, string activePlanId);
        Task<Notification> CreateNotificationAsync(string userId, NotificationType type, string redirectId = null);
        Task ViewNotificationAsync(int notificationId);
        bool CheckUnreadMessageNotificationExistence(string senderId, string recipientId);
        List<NotificationResponseDTO> GetAllByRecipientId(string recipientId);
        Notification GetById(int id);
    }
}
