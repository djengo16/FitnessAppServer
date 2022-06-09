namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.WorkoutDays;

    public interface IWorkoutDaysService
    {
        Task<string> AddAsync(WorkoutDayDTO workoutDay);
    }
}