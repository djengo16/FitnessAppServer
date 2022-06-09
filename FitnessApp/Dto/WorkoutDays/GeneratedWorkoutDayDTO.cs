namespace FitnessApp.Dto.WorkoutDays
{
    using FitnessApp.Dto.ExerciseInWorkoutDay;

    public class GeneratedWorkoutDayDTO
    {
        public string Id { get; set; }
        public DayOfWeek Day { get; set; }

        public virtual ICollection<GeneratedExerciseInWorkoutDayDTO> ExercisesInWorkoutDays { get; set; }
    }
}