namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Notification;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;

    public interface INotificationsService
    {
        bool IsTrainingDayNotification(string userId, string planId);

        Task<Notification> CreateTrainingDayNotificationAsync(string userId);

        Task ViewNotificationAsync(int notificationId);

        Task<NotificationResponseDTO> GetTrainingNotificationAsync(string userId);
    }
}
