using FitnessApp.Models.Common;
using FitnessApp.Models.Enums;

namespace FitnessApp.Models
{
    public class ExerciseInWorkoutDay
    {
        public int ExerciseId { get; set; }
        public virtual Exercise Exercise { get; set; }
        public string WorkoutDayId { get; set; }
        public virtual WorkoutDay WorkoutDay { get; set; }
        public int Sets { get; set; }
        public int MinReps { get; set; }
        public int MaxReps { get; set; }
    }
}