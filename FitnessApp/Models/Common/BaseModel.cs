using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models.Common
{
    public abstract class BaseModel<TKey> : IAuditInfo
    {
        [Key]
        public TKey Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public abstract string WorkoutPlanId { get; set; }
    }
}
