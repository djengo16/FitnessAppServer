namespace FitnessApp.Dto.ExerciseInWorkoutDay
{
    using FitnessApp.Models.Enums;

    public class GeneratedExerciseInWorkoutDayDTO
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public Difficulty Difficulty { get; set; }
        public int Sets { get; set; }
        public int MinReps { get; set; }
        public int MaxReps { get; set; }
        public string Description { get; set; }
        public string PictureResourceUrl { get; set; }
        public string VideoResourceUrl { get; set; }
    }
}