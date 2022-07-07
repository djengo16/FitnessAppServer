using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class WorkoutDay
    {
        public WorkoutDay()
        {
            Id = Guid.NewGuid().ToString();
            ExercisesInWorkoutDays = new HashSet<ExerciseInWorkoutDay>();
        }
        public string Id { get; set; }
        public DayOfWeek Day { get; set; }

        public virtual ICollection<ExerciseInWorkoutDay> ExercisesInWorkoutDays { get; set; }

        public string WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }
    }
}