namespace FitnessApp.Models.Common
{
    public interface IAuditInfo
    {
        DateTime CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }
        string WorkoutPlanId { get; set; }
    }
}
