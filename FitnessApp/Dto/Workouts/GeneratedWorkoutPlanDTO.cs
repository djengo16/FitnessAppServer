namespace FitnessApp.Dto.Workouts
{
    using FitnessApp.Dto.WorkoutDays;
    using FitnessApp.Models.Enums;
    public class GeneratedWorkoutPlanDTO
    {
        public string Id { get; set; }
        public Goal Goal { get; set; }
        public Difficulty Difficulty { get; set; }
        public int DaysInWeek { get; set; }
        public string UserId { get; set; }
        public ICollection<GeneratedWorkoutDayDTO> WorkoutDays { get; set; }
    }
}