namespace FitnessApp.Services.Data
{
    using AutoMapper;
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
            var day = DateTime.Now.DayOfWeek.ToString();

            var notification = new Notification()
            {
                RecipientId = userId,
                Type = NotificationType.TrainingDay,
                IsViewed = false,
                Title = "Training day",
                Body = $"It's {day}, checkout your plan!",
            };

            await notificationsStorage.AddAsync(notification);
            await notificationsStorage.SaveChangesAsync();

            return notification;
        }

        public async Task ViewNotificationAsync(int id)
        {
            var notification = this.notificationsStorage.All().FirstOrDefault(x => x.Id == id);
            notification.IsViewed = true;
            this.notificationsStorage.Update(notification);
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

        public async Task<NotificationResponseDTO> GetTrainingNotificationAsync(string userId)
        {
            var activePlanId = usersService.GetActiveWorkoutPlanId(userId);
            var isTrainingDay = this.IsTrainingDayNotification(userId, activePlanId);

            if (!isTrainingDay)
            {
                return null;
            }

            var notification = notificationsStorage.All().FirstOrDefault(x => x.RecipientId == userId && x.CreatedOn.Date == DateTime.UtcNow.Date);

            if (notification == null)
            {
                notification = await CreateTrainingDayNotificationAsync(userId);
            }

            return mapper.Map<NotificationResponseDTO>(notification);

        }
    }
}