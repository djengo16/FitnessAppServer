namespace FitnessApp.Common
{
    public static class NotificationConstants
    {
        //Training day
        public const string TrainingDayNotificationTitle = "Training day";
        public static string TrainingDayNotificationBody = $"It's {DateTime.Now.DayOfWeek}, checkout your plan!";

        //Unread message
        public const string UnreadMessageNotificationTitle = "New message";

    }
}
