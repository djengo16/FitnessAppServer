namespace FitnessApp.Services.Data
{
    public interface INotificationsService
    {
        bool IsTrainingDayNotification(string userId, string planId);
    }
}
