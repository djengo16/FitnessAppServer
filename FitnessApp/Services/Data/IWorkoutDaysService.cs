namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.WorkoutDays;
    using FitnessApp.Models;

    public interface IWorkoutDaysService
    {
        Task<string> AddAsync(WorkoutDayDTO workoutDay);
        WorkoutDay GetById(string id);
        List<int> GetExerciseIdsInWorkoutDay(string id);
    }
}