namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Workouts;
    public interface IWorkoutsService
    {
        void GenerateWorkoutPlans(WorkoutGenerationInputModel inputModel);

        Task<string> SaveWorkoutPlanAsync(string workoutPlanId);
    }
}