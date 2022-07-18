namespace FitnessApp.Dto.Workouts
{
    using FitnessApp.Models.Enums;
    using FitnessApp.Services.ServiceConstants;
    using System.ComponentModel.DataAnnotations;

    public class WorkoutGenerationInputModel
    {
        [Required(ErrorMessage= ErrorMessages.UserIdIsRequired)]
        public string UserId { get; set; }

        [Required]
        [Range(1, 3, ErrorMessage = ErrorMessages.InvalidWorkoutPlanDifficulty)]
        public Difficulty Difficulty { get; set; }

        [Required]
        [Range(3, 5, ErrorMessage = ErrorMessages.WorkoutPlanDaysNotInRange)]
        public int Days { get; set; }

        [Required]
        [Range(1, 3, ErrorMessage = ErrorMessages.InvalidWorkoutPlanGoal)]
        public Goal Goal { get; set; }

        [Range(0, 10, ErrorMessage = ErrorMessages.WorkoutPlansCountNotInRange)]
        public int Count { get; set; }
    }
}