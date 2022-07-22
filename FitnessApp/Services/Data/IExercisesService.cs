namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Exercises;
    using FitnessApp.Models;

    public interface IExercisesService
    {
        Task CreateExerciseAsync(CreateOrUpdateExerciseDTO exerciseDTO);
        Task UpdateExerciseAsync(int exerciseId, CreateOrUpdateExerciseDTO exerciseDTO);
        GetExerciseDetailsDTO GetExerciseDetails(int exerciseId);
        IEnumerable<ExerciseInListDTO> GetExercises(string searchParams, int? take = null, int skip = 0);
        int GetCount();
        int GetCountBySearchParams(string searchParams);
        Task Delete(int id);
        Exercise GetById(int id);
    }
}