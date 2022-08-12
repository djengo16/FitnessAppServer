using FitnessApp.Dto.ExerciseInWorkoutDay;

namespace FitnessApp.Services.Data
{
    public interface IExerciseInWorkoutDayService
    {
        Task AddAsync(ExerciseInWorkoutDayDTO exerciseInWorkoutDayDTO);

        Task DeleteAllWithExerciseId(int exerciseId);
        Task DeleteExerciseInWorkoutDay(int exerciseId, string workoutId);
        Task<GeneratedExerciseInWorkoutDayDTO> AddExerciseInWorkoutDayAsync(AddExerciseInWorkoutDayDTO dto);
    }
}