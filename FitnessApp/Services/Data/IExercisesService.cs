namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;

    public interface IExercisesService
    {
        ExerciseDTО GetExerciseDetails(int exerciseId);
        IEnumerable<ExerciseInListDTO> GetExercises(string searchParams, int? take = null, int skip = 0);
        int GetCount();
        int GetCountBySearchParams(string searchParams);
        Task Delete(int id);
        Exercise GetById(int id);
        Task CreateAsync(ExerciseInputDTO exercise);
        Task UpdateAsync(ExerciseUpdateDTO exercise);
    }
}