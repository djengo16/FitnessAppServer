namespace FitnessApp.Models
{
    using FitnessApp.Models.Common;
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.SentMessages = new HashSet<Message>();
            this.RecievedMessages = new HashSet<Message>();
            this.UserConversations = new HashSet<UserConversations>();
        }
#nullable enable
        public string? ProfilePicture { get; set; }

        public string? Description { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        [ForeignKey(nameof(WorkoutPlan))]
        public string? WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }

        public virtual ICollection<UserConversations> UserConversations { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> RecievedMessages { get; set; }
    }
}