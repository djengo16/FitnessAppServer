namespace FitnessApp.Dto.Workouts
{
    using FitnessApp.Models.Enums;

    public class WorkoutGenerationInputModel
    {
        public string UserId { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Days { get; set; }
        public Goal Goal { get; set; }
        public int Count { get; set; }
    }
}