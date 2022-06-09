namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Users;
    using FitnessApp.Dto.Workouts;

    public interface IWorkoutsService
    {
        void GenerateWorkoutPlans(WorkoutGenerationInputModel inputModel);

        Task<string> SaveWorkoutPlanAsync(GeneratedWorkoutPlanDTO generatedWorkoutPlan);

        ICollection<GeneratedWorkoutPlanDTO> GetGeneratedWorkoutPlans();
    }
}