namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.WorkoutDays;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;

    public class WorkoutDaysService : IWorkoutDaysService
    {
        private readonly IRepository<WorkoutDay> workoutDays;

        public WorkoutDaysService(IRepository<WorkoutDay> workoutDays)
        {
            this.workoutDays = workoutDays;
        }
        public async Task<string> AddAsync(WorkoutDayDTO workoutDay)
        {
            var currWorkoutDay = new WorkoutDay
            {
                Day = workoutDay.Day,
                WorkoutPlanId = workoutDay.WorkoutPlanId,
            };

            await workoutDays.AddAsync(currWorkoutDay);
            return currWorkoutDay.Id;
        }
    }
}