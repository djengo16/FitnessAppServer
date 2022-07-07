namespace FitnessApp.Models.Common
{
    using System;
    public interface IDeletableEntity
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedOn { get; set; }
        string WorkoutPlanId { get; set; }
    }
}