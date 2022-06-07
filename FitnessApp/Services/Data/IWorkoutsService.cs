namespace FitnessApp.Services.Data
{
    using FitnessApp.Dto.Workouts;
    public interface IWorkoutsService
    {
        void GenerateWorkoutPlan(WorkoutGenerationInputModel inputModel);
        Task SaveWorkoutPlanToUserAsync();
    }
}