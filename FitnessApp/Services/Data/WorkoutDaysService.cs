namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.WorkoutDays;
    using FitnessApp.Models;
    using FitnessApp.Models.Repositories;

    public class WorkoutDaysService : IWorkoutDaysService
    {
        private readonly IRepository<WorkoutDay> workoutDaysStorage;

        public WorkoutDaysService(IRepository<WorkoutDay> workoutDays)
        {
            this.workoutDaysStorage = workoutDays;
        }
        public async Task<string> AddAsync(WorkoutDayDTO workoutDay)
        {
            var currWorkoutDay = new WorkoutDay
            {
                Day = workoutDay.Day,
                WorkoutPlanId = workoutDay.WorkoutPlanId,
            };

            await workoutDaysStorage.AddAsync(currWorkoutDay);
            await workoutDaysStorage.SaveChangesAsync();

            return currWorkoutDay.Id;
        }
    }
}