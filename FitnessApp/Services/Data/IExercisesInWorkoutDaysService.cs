using FitnessApp.Dto.ExerciseInWorkoutDay;

namespace FitnessApp.Services.Data
{
    public interface IExerciseInWorkoutDayService
    {
        Task AddAsync(ExerciseInWorkoutDayDTO exerciseInWorkoutDayDTO);
<<<<<<< Updated upstream
=======
        Task DeleteAllWithExerciseId(int exerciseId);
        Task DeleteExerciseInWorkoutDay(int exerciseId, string workoutId);
>>>>>>> Stashed changes
    }
}