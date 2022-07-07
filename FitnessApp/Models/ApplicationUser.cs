namespace FitnessApp.Models
{
    using FitnessApp.Models.Common;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
#nullable enable
        public string? ProfilePicture { get; set; }

        public string? Description { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        [ForeignKey(nameof(WorkoutPlan))]
        public string? WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }
    }
}