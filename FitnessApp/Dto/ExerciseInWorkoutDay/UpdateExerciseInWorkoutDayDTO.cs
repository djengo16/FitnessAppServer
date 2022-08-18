namespace FitnessApp.Dto.ExerciseInWorkoutDay
{
    public class UpdateExerciseInWorkoutDayDTO
    {
        public int ExerciseId { get; set; }
        public string WorkoutDayId { get; set; }
        public int Sets { get; set; }
        public int MinReps { get; set; }
        public int MaxReps { get; set; }
    }
}