namespace FitnessApp.Models
{
    public class WorkoutDay
    {
        public WorkoutDay()
        {
            Id = Guid.NewGuid().ToString();
            Exercises = new HashSet<ExerciseInWorkoutDay>();
            ExerciseInWorkoutDay = new HashSet<ExerciseInWorkoutDay>();
        }
        public string Id { get; set; }
        public DayOfWeek Day { get; set; }

        public virtual ICollection<ExerciseInWorkoutDay> Exercises { get; set; }
        public virtual ICollection<ExerciseInWorkoutDay> ExerciseInWorkoutDay { get; set; }

        public string WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }
    }
}