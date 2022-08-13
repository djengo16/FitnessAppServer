using FitnessApp.Models.Common;
using FitnessApp.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class Notification : IAuditInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsViewed { get; set; }
        public NotificationType Type { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [ForeignKey("User")]
        public string RecipientId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
