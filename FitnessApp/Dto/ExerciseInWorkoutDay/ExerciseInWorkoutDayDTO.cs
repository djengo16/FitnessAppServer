using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Dto.ExerciseInWorkoutDay
{
    public class ExerciseInWorkoutDayDTO
    {
        [Required]
        public int ExerciseId { get; set; }

        [Required]
        public string WorkoutDayId { get; set; }

        [Required(ErrorMessage = ValidationMessages.SetsAreRequired)]
        [Range(1, 100, ErrorMessage = ValidationMessages.SetsAreNotInRange)]
        public int Sets { get; set; }

        [Required(ErrorMessage = ValidationMessages.MinRepsAreRequired)]
        [Range(1, 100, ErrorMessage = ValidationMessages.MinRepsAreNotInRange)]
        public int MinReps { get; set; }

        [Required(ErrorMessage = ValidationMessages.MaxRepsAreRequired)]
        [Range(1, 100, ErrorMessage = ValidationMessages.MaxRepsAreNotInRage)]
        public int MaxReps { get; set; }
    }
}
