namespace FitnessApp.Helper.Builders.Notification
{
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;

    public class NotificationBuilder : INotificationBuilder
    {
        private Notification notification;

        public NotificationBuilder()
        {
            this.notification = new Notification();
        }

        public void BuildBody(string body) => notification.Body = body;

        public void BuildNotificationType(NotificationType type) => notification.Type = type;

        public void BuildRecipientId(string recipientId) => notification.RecipientId = recipientId;

        public void BuildRedirectId(string redirectId) => notification.RedirectId = redirectId;

        public void BuildTitle(string title) => notification.Title = title;

        public Notification Build()
        {
            this.notification.IsViewed = false;
            return notification;
        }
    }
}
