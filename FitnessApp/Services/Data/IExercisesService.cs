namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Exercises;

    public interface IExercisesService
    {
        Task CreateExerciseAsync(CreateOrUpdateExerciseDTO exerciseDTO);
        Task UpdateExerciseAsync(int exerciseId, CreateOrUpdateExerciseDTO exerciseDTO);
        GetExerciseDetailsDTO GetExerciseDetails(int exerciseId);
    }
}