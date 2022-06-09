namespace FitnessApp.Dto.Users
{
    using FitnessApp.Dto.Workouts;

    public class AssignWorkoutPlanToUserDTO
    {
        public string UserId { get; set; }

        public GeneratedWorkoutPlanDTO Wrokout { get; set; }
    }
}