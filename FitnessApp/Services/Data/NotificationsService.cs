namespace FitnessApp.Services.Data
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using FitnessApp.Common;
    using FitnessApp.Dto.Notification;
    using FitnessApp.Models;
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Repositories;


    public class NotificationsService : INotificationsService
    {
        private readonly IWorkoutsService workoutsService;
        private readonly IRepository<Notification> notificationsStorage;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public NotificationsService(
            IWorkoutsService workoutsService, 
            IRepository<Notification> notificationsStorage, 
            IUsersService usersService, 
            IMapper mapper)
        {
            this.workoutsService = workoutsService;
            this.notificationsStorage = notificationsStorage;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        public async Task<Notification> CreateTrainingDayNotificationAsync(string userId)
        {

            var notification = new Notification()
            {
                RecipientId = userId,
                Type = NotificationType.TrainingDay,
                IsViewed = false,
                Title = NotificationConstants.TrainingDayNotificationTitle,
                Body = NotificationConstants.TrainingDayNotificationBody,
            };

            await notificationsStorage.AddAsync(notification);
            await notificationsStorage.SaveChangesAsync();

            return notification;
        }

        public async Task<Notification> CreateUnreadMessageNotification(string recipientId, string senderId)
        {
            var senderEmail = await this.usersService.GetUserEmailAsync(senderId);
            var notificationBody = $"You have unread messages from {senderEmail}.";

            var notification = new Notification()
            {
                RecipientId = recipientId,
                RedirectId = senderId,
                Type = NotificationType.UnreadMessage,
                IsViewed = false,
                Title = NotificationConstants.UnreadMessageNotificationTitle,
                Body = notificationBody,
            };

            await notificationsStorage.AddAsync(notification);
            await notificationsStorage.SaveChangesAsync();

            return notification;
        }

        /// <summary>
        /// Sets the notification property IsViewed to true and deletes the notification if it's of type
        /// UnreadMessage (we don't need to keep it), updates it's othervise.
        /// </summary>
        /// <param name="id">Notification id</param>
        /// <returns></returns>
        public async Task ViewNotificationAsync(int id)
        {
            var notification = this.notificationsStorage.All().FirstOrDefault(x => x.Id == id);
            notification.IsViewed = true;

            if(notification.Type == NotificationType.UnreadMessage)
            {
                this.notificationsStorage.Delete(notification);
            }
            else
            {
                this.notificationsStorage.Update(notification);
            }
            await this.notificationsStorage.SaveChangesAsync();
        }


        public bool IsTrainingDayNotification(string userId, string planId)
        {
            var workoutPlan = workoutsService.GetUserWorkoutPlan(userId, planId);

            foreach (var day in workoutPlan.WorkoutDays)
            {
                if (day.Day == DateTime.Today.DayOfWeek)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if unread message notification already exists for this sender and recipient.
        /// </summary>
        /// <param name="senderId">The user that another user received messages from</param>
        /// <param name="recipientId">The notification receiver</param>
        /// <returns>True if notification exists and false if does not.</returns>
        public bool CheckUnreadMessageNotificationExistence(string senderId, string recipientId)
        {
            var notification = this.notificationsStorage
                 .All()
                 .Where(x => x.IsViewed == false
                 && x.RedirectId == senderId
                 && x.RecipientId == recipientId)
                 .FirstOrDefault();

            return notification != null;
        }
        public async Task<List<NotificationResponseDTO>> GetAllByRecipientId(string recipientId)
        {
            var notifications = new List<NotificationResponseDTO>();
            var trainingDayNotification = await this.GetTrainingNotificationAsync(recipientId);
            var unreadMessageNotification = this.GetUnreadMessageNotifications(recipientId);

            if(trainingDayNotification != null)
            {
                notifications.Add(trainingDayNotification);
            }

            if(unreadMessageNotification != null)
            {
                notifications.AddRange(unreadMessageNotification);
            }

            return notifications;
        }
        private List<NotificationResponseDTO> GetUnreadMessageNotifications(string userId)
        {
            var notifications = this.notificationsStorage
                .All()
                .Where(x => x.RecipientId == userId
                 && x.Type == NotificationType.UnreadMessage
                 && x.IsViewed == false)
                .ProjectTo<NotificationResponseDTO>(this.mapper.ConfigurationProvider)
                .ToList();

            return notifications;
        }

        private async Task<NotificationResponseDTO> GetTrainingNotificationAsync(string userId)
        {
            var activePlanId = usersService.GetActiveWorkoutPlanId(userId);

            if(activePlanId == null)
            {
                return null;
            }

            var isTrainingDay = this.IsTrainingDayNotification(userId, activePlanId);

            if (!isTrainingDay)
            {
                return null;
            }

            var notification = notificationsStorage
                .All()
                .FirstOrDefault(x => x.RecipientId == userId 
                && x.CreatedOn.Date == DateTime.UtcNow.Date
                && x.Title == NotificationConstants.TrainingDayNotificationTitle);

            if (notification != null && notification.IsViewed)
            { 
                return null;
            }

            if (notification == null)
            {
                notification = await CreateTrainingDayNotificationAsync(userId);
            }


            return mapper.Map<NotificationResponseDTO>(notification);

        }
    }
}