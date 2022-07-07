namespace FitnessApp.Models
{
    using FitnessApp.Models.Enums;
    using FitnessApp.Models.Common;
    using System.ComponentModel.DataAnnotations.Schema;

    public class WorkoutPlan : BaseDeletableModel<string>
    {
        public WorkoutPlan()
        {
            Id = Guid.NewGuid().ToString();
            WorkoutDays = new HashSet<WorkoutDay>();
        }
        public WorkoutPlan(string id)
        {
            Id = id;
            WorkoutDays = new HashSet<WorkoutDay>();
        }
        public Goal Goal { get; set; }
        public Difficulty Difficulty { get; set; }
        public int DaysInWeek => WorkoutDays.Count;

        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public virtual ICollection<WorkoutDay> WorkoutDays { get; set; }
        
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}