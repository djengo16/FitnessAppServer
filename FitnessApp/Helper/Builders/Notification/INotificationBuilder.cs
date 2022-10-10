namespace FitnessApp.Helper.Builders.Notification
{
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;

    public interface INotificationBuilder
    {
        void BuildTitle(string title);
        void BuildBody(string body);
        void BuildNotificationType(NotificationType type);
        void BuildRecipientId(string recipientId);
        void BuildRedirectId(string redirectId);
        Notification Build();
    }
}
