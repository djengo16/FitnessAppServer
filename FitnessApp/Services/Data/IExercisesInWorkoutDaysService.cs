using FitnessApp.Dto.ExerciseInWorkoutDay;

namespace FitnessApp.Services.Data
{
    public interface IExerciseInWorkoutDayService
    {
        Task AddAsync(ExerciseInWorkoutDayDTO exerciseInWorkoutDayDTO);
    }
}