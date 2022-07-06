namespace FitnessApp.Services.Data
{

    public class NotificationsService : INotificationsService
    {
        private readonly IWorkoutsService workoutsService;

        public NotificationsService(IWorkoutsService workoutsService)
        {
            this.workoutsService = workoutsService;
        }

        public bool IsTrainingDayNotification(string userId)
        {
            var workoutPlan =  workoutsService.GetUserWorkoutPlan(userId);

            foreach (var day in workoutPlan.WorkoutDays)
            {
                if(day.Day == DateTime.Today.DayOfWeek)
                {
                    return true;
                }
            }

            return false;
        }
    }
}