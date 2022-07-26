namespace FitnessApp.Dto.Workouts
{
    public class UserWorkoutPlanInAllUserPlansDTO
    {
        public string Id { get; set; }
        public bool IsActive { get; set; } = false;
        public string Difficulty { get; set; }
        public int WorkoutDays { get; set; }
        public string Goal { get; set; }
    }
}