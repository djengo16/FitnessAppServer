using FitnessApp.Dto.ExerciseInWorkoutDay;

namespace FitnessApp.Services.Data
{
    public interface IExerciseInWorkoutDayService
    {
        Task AddAsync(ExerciseInWorkoutDayDTO exerciseInWorkoutDayDTO);

        Task DeleteAllWithExerciseIdAsync(int exerciseId);
        Task DeleteAsync(int exerciseId, string workoutId);
        Task<GeneratedExerciseInWorkoutDayDTO> AddAsync(AddExerciseInWorkoutDayDTO dto);
        Task UpdateRangeAsync(ICollection<UpdateExerciseInWorkoutDayDTO> dto);
    }
}