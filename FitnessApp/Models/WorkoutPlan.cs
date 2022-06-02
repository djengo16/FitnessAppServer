namespace FitnessApp.Models
{
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Common;
    public class WorkoutPlan : BaseDeletableModel<string>
    {
        public WorkoutPlan()
        {
            Id = Guid.NewGuid().ToString();
            WorkoutDays = new HashSet<WorkoutDay>();
        }
        public Goal Goal { get; set; }
        public virtual ICollection<WorkoutDay> WorkoutDays { get; set; }
    }
}
