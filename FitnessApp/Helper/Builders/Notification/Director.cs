namespace FitnessApp.Helper.Builders.Notification
{
    using FitnessApp.Common;
    using FitnessApp.Models.Enums;
    using FitnessApp.Services.Data;

    public class Director
    {
        private readonly INotificationBuilder notificationBuilder;
        private readonly IUsersService usersService;

        public Director(INotificationBuilder notificationBuilder, IUsersService usersService)
        {
            this.notificationBuilder = notificationBuilder;
            this.usersService = usersService;
        }

        public INotificationBuilder NotificationBuilder => notificationBuilder;

        public void BuildTrainingDayNotification(string recipientId)
        {
            this.notificationBuilder.BuildRecipientId(recipientId);
            this.notificationBuilder.BuildNotificationType(NotificationType.TrainingDay);
            this.notificationBuilder.BuildTitle(NotificationConstants.TrainingDayNotificationTitle);
            this.notificationBuilder.BuildBody(NotificationConstants.TrainingDayNotificationBody);
        }
        public async Task BuildUnreadMessageNotificationAsync(string recipientId, string senderId)
        {
            var senderName = await this.usersService.GetNameAsync(senderId);
            var notificationBody = $"You have unread messages from {senderName}.";

            this.notificationBuilder.BuildRecipientId(recipientId);
            this.notificationBuilder.BuildNotificationType(NotificationType.UnreadMessage);
            this.notificationBuilder.BuildTitle(NotificationConstants.UnreadMessageNotificationTitle);
            this.notificationBuilder.BuildBody(notificationBody);
            this.notificationBuilder.BuildRedirectId(senderId);
        }
    }
}
