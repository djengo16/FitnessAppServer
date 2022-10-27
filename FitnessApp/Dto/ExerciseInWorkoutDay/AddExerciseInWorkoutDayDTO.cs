namespace FitnessApp.Dto.ExerciseInWorkoutDay
{
    using System.ComponentModel.DataAnnotations;

    public class AddExerciseToExistingDayDTO
    {
        [Required]
        public int ExerciseId { get; set; }

        [Required]
        public string WorkoutDayId { get; set; }
    }
}
