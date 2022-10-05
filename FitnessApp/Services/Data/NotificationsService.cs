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
        /// <summary>
        /// Checks if it's training day.
        /// Checks if notification of type training day already exist.
        /// Creates new notification if the checks are passed.
        /// </summary>
        /// <param name="userId">Target user</param>
        /// <param name="trainingDayId">User's training day id</param>
        /// <returns></returns>
        public async Task<Notification> SetupTrainingDayNotification(string userId, string activePlanId)
        {
            bool itsTrainingDay = workoutsService.IsTrainingDay(userId, activePlanId);

            if (!itsTrainingDay)
            {
                return null;
            }

            bool itsExisting = DoesUnreadNotificationExist(NotificationType.TrainingDay, userId);

            if (itsExisting)
            {
                return null;
            }

            var notification = await CreateTrainingDayNotificationAsync(userId);

            return notification;
        }

        public bool DoesUnreadNotificationExist(NotificationType type, string userId)
        {
            var notification = notificationsStorage
                .All()
                .FirstOrDefault(x => x.RecipientId == userId
                && x.CreatedOn.Date == DateTime.UtcNow.Date
                && x.IsViewed == false
                && x.Type == type);

            return notification != null;
        }

        public async Task<Notification> CreateUnreadMessageNotification(string recipientId, string senderId)
        {
            var senderName = await this.usersService.GetNameAsync(senderId);
            var notificationBody = $"You have unread messages from {senderName}.";

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
            var notifications = notificationsStorage
                .All()
                .Where(x => x.RecipientId == recipientId
                 && x.IsViewed == false)
                .OrderBy(x => x.CreatedOn)
                .ProjectTo<NotificationResponseDTO>(this.mapper.ConfigurationProvider)
                .ToList();


            return notifications;
        }
    }
}